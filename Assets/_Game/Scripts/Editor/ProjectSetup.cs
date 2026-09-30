using System.Collections.Generic;
using System.IO;
using NationsWars.Game;
using NationsWars.Sim;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace NationsWars.EditorTools
{
    /// <summary>
    /// Nations Wars menu: creates the starter nations, units and weapons as assets, and builds the Battle scene.
    /// Safe to run again: existing assets are reused and the scene is rebuilt.
    /// </summary>
    public static class ProjectSetup
    {
        const string Root = "Assets/_Game";
        const string ScenePath = Root + "/Scenes/Battle.unity";

        [MenuItem("Nations Wars/Setup Skirmish Prototype")]
        public static void SetupAll()
        {
            NationDef[] nations = CreateSampleContent();
            BuildBattleScene(nations);
            Debug.Log("Nations Wars: prototype ready. Press Play in the Battle scene.");
        }

        [MenuItem("Nations Wars/Create Sample Nations Only")]
        public static void CreateSampleNationsOnly()
        {
            CreateSampleContent();
        }

        // ---------------------------------------------------------------- content

        static NationDef[] CreateSampleContent()
        {
            WeaponDef rifle = Weapon("Rifle", Warhead.Bullet, 8, 10f, 0.5f);
            WeaponDef cannon = Weapon("Tank Cannon", Warhead.Cannon, 40, 18f, 1.6f);

            // Atlantic Alliance: balanced, air power later
            var aaRanger = UnitAsset("AtlanticAlliance", "Ranger", UnitRole.Infantry, 100, 100, 3.5f, Armor.Infantry, rifle);
            var aaGrizzly = UnitAsset("AtlanticAlliance", "Grizzly MBT", UnitRole.Tank, 900, 400, 6f, Armor.HeavyArmor, cannon);
            var aaHarvester = UnitAsset("AtlanticAlliance", "Harvester", UnitRole.Harvester, 1400, 600, 4.5f, Armor.LightVehicle, null);

            NationDef atlantic = Nation("AtlanticAlliance", "Atlantic Alliance", "Air power & precision", new Color(0.18f, 0.48f, 0.94f),
                "Radar range +25%. Aircraft cost 15% less.", "Orbital Strike",
                new List<UnitDef> { aaRanger, aaGrizzly, aaHarvester },
                new List<BuildingDef>
                {
                    Building("AtlanticAlliance", "Construction Yard", 3000, 1500, new Vector2Int(4, 4), 0, 0, false),
                    Building("AtlanticAlliance", "Power Plant", 600, 700, new Vector2Int(3, 3), 100, 0, false),
                    Building("AtlanticAlliance", "Ore Refinery", 2000, 900, new Vector2Int(4, 3), 0, 30, false),
                    Building("AtlanticAlliance", "Barracks", 500, 600, new Vector2Int(3, 3), 0, 20, false),
                    Building("AtlanticAlliance", "War Factory", 2000, 1000, new Vector2Int(4, 4), 0, 40, false),
                    Building("AtlanticAlliance", "Airbase", 1600, 900, new Vector2Int(4, 4), 0, 50, true),
                },
                new List<UnitDef> { aaHarvester, aaGrizzly, aaGrizzly, aaGrizzly, aaGrizzly, aaRanger, aaRanger, aaRanger, aaRanger, aaRanger });

            // Red Star Union: tanks have +15% HP and -10% speed
            var rsConscript = UnitAsset("RedStarUnion", "Conscript", UnitRole.Infantry, 100, 90, 3.4f, Armor.Infantry, rifle);
            var rsRhino = UnitAsset("RedStarUnion", "Rhino Tank", UnitRole.Tank, 900, 460, 5.4f, Armor.HeavyArmor, cannon);
            var rsHarvester = UnitAsset("RedStarUnion", "Harvester", UnitRole.Harvester, 1400, 600, 4.5f, Armor.LightVehicle, null);

            NationDef redStar = Nation("RedStarUnion", "Red Star Union", "Heavy armor & Tesla tech", new Color(0.85f, 0.25f, 0.18f),
                "Tanks have +15% HP and -10% speed. Units inside a Tesla field self-repair.", "Iron Curtain",
                new List<UnitDef> { rsConscript, rsRhino, rsHarvester },
                new List<BuildingDef>
                {
                    Building("RedStarUnion", "Construction Yard", 3000, 1500, new Vector2Int(4, 4), 0, 0, false),
                    Building("RedStarUnion", "Tesla Reactor", 600, 700, new Vector2Int(3, 3), 100, 0, true),
                    Building("RedStarUnion", "Ore Refinery", 2000, 900, new Vector2Int(4, 3), 0, 30, false),
                    Building("RedStarUnion", "Barracks", 500, 600, new Vector2Int(3, 3), 0, 20, false),
                    Building("RedStarUnion", "War Factory", 2000, 1000, new Vector2Int(4, 4), 0, 40, false),
                    Building("RedStarUnion", "Tesla Coil", 1200, 800, new Vector2Int(2, 2), 0, 75, true),
                },
                new List<UnitDef> { rsHarvester, rsRhino, rsRhino, rsRhino, rsRhino, rsConscript, rsConscript, rsConscript, rsConscript, rsConscript });

            AssetDatabase.SaveAssets();
            return new[] { atlantic, redStar };
        }

        static WeaponDef Weapon(string name, Warhead warhead, int damage, float range, float cooldown)
        {
            var w = LoadOrCreate<WeaponDef>(Root + "/Data/Weapons/" + name + ".asset");
            w.displayName = name; w.warhead = warhead; w.damage = damage; w.range = range; w.cooldown = cooldown;
            EditorUtility.SetDirty(w);
            return w;
        }

        static UnitDef UnitAsset(string nationFolder, string name, UnitRole role, int cost, int hp, float speed, Armor armor, WeaponDef weapon)
        {
            var u = LoadOrCreate<UnitDef>(Root + "/Data/Nations/" + nationFolder + "/Units/" + name + ".asset");
            u.displayName = name; u.role = role; u.cost = cost; u.maxHealth = hp; u.moveSpeed = speed; u.armor = armor; u.weapon = weapon;
            u.buildTime = Mathf.Max(3f, cost / 100f);
            EditorUtility.SetDirty(u);
            return u;
        }

        static BuildingDef Building(string nationFolder, string name, int cost, int hp, Vector2Int footprint, int powerOut, int powerDraw, bool unique)
        {
            var b = LoadOrCreate<BuildingDef>(Root + "/Data/Nations/" + nationFolder + "/Buildings/" + name + ".asset");
            b.displayName = name; b.cost = cost; b.maxHealth = hp; b.footprint = footprint;
            b.powerOutput = powerOut; b.powerDraw = powerDraw; b.nationUnique = unique; b.buildTime = Mathf.Max(5f, cost / 100f);
            EditorUtility.SetDirty(b);
            return b;
        }

        static NationDef Nation(string folder, string name, string tagline, Color color, string passive, string superweapon,
            List<UnitDef> units, List<BuildingDef> buildings, List<UnitDef> starting)
        {
            var n = LoadOrCreate<NationDef>(Root + "/Data/Nations/" + folder + "/" + folder + ".asset");
            n.displayName = name; n.tagline = tagline; n.color = color; n.passive = passive; n.superweapon = superweapon;
            n.units = units; n.buildings = buildings; n.startingUnits = starting;
            EditorUtility.SetDirty(n);
            return n;
        }

        static T LoadOrCreate<T>(string path) where T : ScriptableObject
        {
            var existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing != null) return existing;

            EnsureFolder(Path.GetDirectoryName(path).Replace('\\', '/'));
            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        static void EnsureFolder(string folder)
        {
            if (AssetDatabase.IsValidFolder(folder)) return;
            string parent = Path.GetDirectoryName(folder).Replace('\\', '/');
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(folder));
        }

        // ---------------------------------------------------------------- scene

        static void BuildBattleScene(NationDef[] nations)
        {
            EnsureFolder(Root + "/Scenes");
            EnsureFolder(Root + "/Materials");

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var sun = new GameObject("Sun");
            var light = sun.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.1f;
            light.shadows = LightShadows.Soft;
            sun.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.55f, 0.6f, 0.66f);

            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.localScale = new Vector3(25.6f, 1f, 25.6f); // a plane is 10 units wide: 256 x 256
            ground.GetComponent<Renderer>().sharedMaterial = SaveMaterial("Ground", new Color(0.36f, 0.5f, 0.24f));

            var camGo = new GameObject("Main Camera") { tag = "MainCamera" };
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.62f, 0.75f, 0.88f);
            cam.farClipPlane = 600f;
            camGo.AddComponent<AudioListener>();
            var rig = camGo.AddComponent<RtsCameraController>();

            var game = new GameObject("Game");
            game.AddComponent<MapGrid>();
            var boot = game.AddComponent<SkirmishBootstrap>();
            boot.nations = nations;
            boot.cameraController = rig;
            game.AddComponent<SelectionManager>();
            game.AddComponent<CommandIssuer>();
            game.AddComponent<SkirmishHud>();

            var props = new GameObject("Obstacles");
            Material rock = SaveMaterial("Rock", new Color(0.5f, 0.5f, 0.48f));
            // A wall across the middle with a gap on the right, so pathfinding has something to route around.
            for (int i = 0; i < 6; i++) Obstacle(props, rock, new Vector3(-50f + i * 10f, 2f, 0f), new Vector3(10f, 4f, 3f));
            Obstacle(props, rock, new Vector3(-40f, 1.5f, 40f), new Vector3(8f, 3f, 8f));
            Obstacle(props, rock, new Vector3(35f, 1.5f, -35f), new Vector3(8f, 3f, 8f));
            Obstacle(props, rock, new Vector3(0f, 1.5f, 25f), new Vector3(12f, 3f, 4f));

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        }

        static void Obstacle(GameObject parent, Material mat, Vector3 pos, Vector3 size)
        {
            var o = GameObject.CreatePrimitive(PrimitiveType.Cube);
            o.name = "Obstacle";
            o.transform.SetParent(parent.transform, false);
            o.transform.position = pos;
            o.transform.localScale = size;
            o.GetComponent<Renderer>().sharedMaterial = mat;
            o.AddComponent<MapObstacle>();
        }

        static Material SaveMaterial(string name, Color color)
        {
            string path = Root + "/Materials/" + name + ".mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null) { existing.color = color; return existing; }
            var m = MaterialUtil.Create(color);
            AssetDatabase.CreateAsset(m, path);
            return m;
        }
    }
}
