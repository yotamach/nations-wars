using System;
using System.Collections.Generic;
using NationsWars.Sim;
using UnityEngine;

namespace NationsWars.Game
{
    /// <summary>
    /// A controllable unit: moves along an A* path, auto-fires at enemies in range, and earns stars for kills.
    /// </summary>
    public class Unit : MonoBehaviour
    {
        public static readonly List<Unit> All = new List<Unit>();

        /// <summary>Raised when any unit gains a star. Arguments: the unit and its new star count.</summary>
        public static event Action<Unit, int> AnyPromoted;

        const float ScanInterval = 0.25f;
        const float RepathInterval = 0.5f;

        public UnitDef Def { get; private set; }
        public int OwnerSlot { get; private set; }
        public int Health { get; private set; }
        public bool IsSelected { get; private set; }
        public bool IsDead { get; private set; }
        public int Kills { get; private set; }
        public Veterancy Vet { get; private set; }

        public int Stars { get { return Vet.Stars; } }
        public bool IsMoving { get { return pathIndex < path.Count; } }

        readonly List<GridPos> path = new List<GridPos>();
        int pathIndex;
        GameObject selectionRing;

        Unit forcedTarget;
        Unit autoTarget;
        float cooldown;
        float scanTimer;
        float repathTimer;
        float healRemainder;

        void OnEnable() { All.Add(this); }
        void OnDisable() { All.Remove(this); }

        public void Init(UnitDef def, int ownerSlot, GameObject ring)
        {
            Def = def;
            OwnerSlot = ownerSlot;
            Health = def.maxHealth;
            Vet = new Veterancy(def.cost);
            selectionRing = ring;
            SetSelected(false);
        }

        public void SetSelected(bool selected)
        {
            IsSelected = selected;
            if (selectionRing != null) selectionRing.SetActive(selected);
        }

        // ------------------------------------------------------------ orders

        /// <summary>Move order. Cancels any attack order.</summary>
        public void MoveTo(Vector3 world)
        {
            forcedTarget = null;
            PathTo(world);
        }

        /// <summary>Attack order. Moves into weapon range and fires until the target dies or another order is given.</summary>
        public void OrderAttack(Unit target)
        {
            if (target == null || target.IsDead || Def.weapon == null) return;
            forcedTarget = target;
            repathTimer = 0f;
        }

        void PathTo(Vector3 world)
        {
            path.Clear();
            pathIndex = 0;

            MapGrid map = MapGrid.Instance;
            if (map == null) return;

            List<GridPos> found = AStar.FindPath(map.Grid, map.WorldToCell(transform.position), map.WorldToCell(world));
            if (found != null) path.AddRange(found);
        }

        void StopMoving()
        {
            path.Clear();
            pathIndex = 0;
        }

        // ------------------------------------------------------------ combat

        /// <summary>Applies damage that was already scaled by armor and ranks. Credits the attacker if this kills.</summary>
        public void TakeDamage(int amount, Unit attacker)
        {
            if (IsDead || amount <= 0) return;
            Health -= amount;
            if (Health <= 0) Die(attacker);
        }

        void Die(Unit killer)
        {
            IsDead = true;
            All.Remove(this);
            if (killer != null && !killer.IsDead) killer.RegisterKill(this);
            Destroy(gameObject);
        }

        void RegisterKill(Unit victim)
        {
            Kills++;
            if (!Vet.AddKill(victim.Def.cost)) return;

            Action<Unit, int> handler = AnyPromoted;
            if (handler != null) handler(this, Vet.Stars);
        }

        void Fire(Unit target)
        {
            WeaponDef w = Def.weapon;
            int damage = DamageTable.Default.Calculate(w.damage, w.warhead, target.Def.armor, Stars, target.Stars);
            cooldown = w.cooldown;

            Vector3 from = transform.position + Vector3.up * 1.4f;
            Vector3 to = target.transform.position + Vector3.up * 1.0f;
            Tracer.Spawn(from, to, w.warhead);
            target.TakeDamage(damage, this);
        }

        Unit FindTargetInRange()
        {
            float range = Def.weapon.range;
            float best = range * range;
            Unit found = null;

            foreach (Unit u in All)
            {
                if (u == this || u.IsDead || !SkirmishSession.AreEnemies(OwnerSlot, u.OwnerSlot)) continue;
                float d = FlatSqrDistance(u);
                if (d <= best) { best = d; found = u; }
            }
            return found;
        }

        float FlatSqrDistance(Unit other)
        {
            Vector3 d = other.transform.position - transform.position;
            d.y = 0f;
            return d.sqrMagnitude;
        }

        void FaceTowards(Vector3 worldPoint)
        {
            Vector3 dir = worldPoint - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.0001f) return;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), 360f * Time.deltaTime);
        }

        void HealOverTime()
        {
            float fraction = VeterancyRules.HealFractionPerSecond(Stars);
            if (fraction <= 0f || Health >= Def.maxHealth) return;

            healRemainder += Def.maxHealth * fraction * Time.deltaTime;
            int whole = (int)healRemainder;
            if (whole <= 0) return;
            healRemainder -= whole;
            Health = Mathf.Min(Def.maxHealth, Health + whole);
        }

        // ------------------------------------------------------------ update

        void Update()
        {
            if (IsDead) return;

            HealOverTime();
            cooldown -= Time.deltaTime;

            if (Def.weapon != null && UpdateCombat()) return;
            Move();
        }

        /// <summary>Returns true when the unit is standing and fighting this frame, so it should not move.</summary>
        bool UpdateCombat()
        {
            if (forcedTarget != null && forcedTarget.IsDead) forcedTarget = null;

            Unit target = forcedTarget;
            if (target == null && !IsMoving)
            {
                scanTimer -= Time.deltaTime;
                if (scanTimer <= 0f || (autoTarget != null && autoTarget.IsDead))
                {
                    scanTimer = ScanInterval;
                    autoTarget = FindTargetInRange();
                }
                target = autoTarget;
                if (target != null && (target.IsDead || FlatSqrDistance(target) > Def.weapon.range * Def.weapon.range))
                    target = autoTarget = null;
            }
            if (target == null) return false;

            float range = Def.weapon.range;
            if (FlatSqrDistance(target) <= range * range)
            {
                if (forcedTarget != null) StopMoving();
                FaceTowards(target.transform.position);
                if (cooldown <= 0f) Fire(target);
                return true;
            }

            // Chasing a forced target: re-plan now and then because it moves.
            repathTimer -= Time.deltaTime;
            if (repathTimer <= 0f)
            {
                repathTimer = RepathInterval;
                PathTo(target.transform.position);
            }
            return false;
        }

        void Move()
        {
            if (!IsMoving) return;

            Vector3 target = MapGrid.Instance.CellToWorld(path[pathIndex]);
            target.y = transform.position.y;

            Vector3 to = target - transform.position;
            float dist = to.magnitude;
            float step = Def.moveSpeed * Time.deltaTime;

            if (dist <= step)
            {
                transform.position = target;
                pathIndex++;
                return;
            }

            Vector3 dir = to / dist;
            transform.position += dir * step;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), 360f * Time.deltaTime);
        }
    }
}
