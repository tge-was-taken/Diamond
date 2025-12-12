using Diamond;
using Diamond.FileFormats;
using System.Text;

if (args.Length < 2)
{
    Console.WriteLine("Usage: Diamond.WikiGenCmd <MH data root path> <DO data root path>");
    return;
}

var root = args[0];
var doRoot = args[1];

Console.WriteLine($"root: {root}, doroot: {doRoot}");

var mobsScr = MobsScrFile2.Load(Path.Combine(root, @"data\script\mobs.scr"));
var itemsScr = ItemsScrFile.Load(Path.Combine(root, @"data\script\items.scr"));
var npcsScr = NpcsScrFile.Load(Path.Combine(root, @"data\script\npcs.scr"));
var questsScr = QuestScrFile.Load(Path.Combine(root, @"data\script\quests.scr"));

var regionFiles = new List<RegionFile>();
foreach (var file in Directory.EnumerateFiles(Path.Combine(doRoot, @"data"), "region???.bin", SearchOption.AllDirectories))
{
    var region = RegionFile.Load(file);
    regionFiles.Add(region);
}
var regionTableFiles = new List<RegionTableFile>();
foreach (var file in Directory.EnumerateFiles(Path.Combine(doRoot, @"data"), "regiontable???.bin", SearchOption.AllDirectories))
{
    var regionTableFile = RegionTableFile.Load(file);
    regionTableFiles.Add(regionTableFile);
}
foreach (var file in Directory.EnumerateFiles(Path.Combine(root, @"data"), "regiontable???.bin", SearchOption.AllDirectories))
{
    var regionTableFile = RegionTableFile.Load(file);
    regionTableFiles.RemoveAll(r => r.MapId == regionTableFile.MapId);
    regionTableFiles.Add(regionTableFile);
}

var mobArrFiles = new List<MobArrFile>();
foreach (var file in Directory.EnumerateFiles(Path.Combine(doRoot, @"data"), "mob???.arr", SearchOption.AllDirectories))
{
    var mobArr = MobArrFile.Load(file);
    mobArrFiles.Add(mobArr);
}
var npcArrFiles = new List<NpcArrFile>();
foreach (var file in Directory.EnumerateFiles(Path.Combine(doRoot, @"data"), "npc???.arr", SearchOption.AllDirectories))
{
    var npcArr = NpcArrFile.Load(file);
    npcArrFiles.Add(npcArr);
}

var mapNames = new Dictionary<int, string>()
    {
        { 0, "Selection Screen" },
        { 1, "Valley Town" },
        { 2, "Kingdom County" },
        { 3, "Deadland" },
        { 4, "Tibet" },
        { 5, "Ancient Tomb Lv.3" },
        { 6, "Training Camp" },
        { 7, "Red Castle" },
        { 8, "Mid Castle" },
        { 9, "Skyzone" },
        { 10, "144 Castle" },
        { 11, "144 Cave" },
        { 12, "160 Castle" },
        { 13, "Canyon of Despair" },
        { 14, "Zenko-ji Temple" },
        { 15, "Buddhist Underground" },
        { 16, "Khao Luang Cave" },
        { 17, "Beyul" },
        { 18, "Shangri-La" },
        { 19, "Han Dynasty" },
        { 20, "Tang Dynasty" },
        { 21, "Bagan" },
        { 22, "Naraka Cave" },
        { 23, "Nothingness" },
        { 24, "Nothingness" },
        { 25, "Hwaryong's Hideout" },
        { 26, "Trial Tower - Floor 1" },
        { 27, "시련의탑 1층(정원)" },
        { 28, "시련의탑2층" },
        { 29, "도전의 성도 4구역" },
        { 30, "시련의탑4층" },
        { 31, "시련의 탑 5층" },
        { 32, "도전의성도 1구역" },
        { 33, "시련의탑6층" },
        { 34, "도전의성도3구역" },
        { 35, "투쟁의 탑" },
        { 36, "시련의탑 8층" },
        { 37, "투쟁의탑" },
        { 38, "시련의탑9층" },
        { 39, "투쟁의탑10구역" },
        { 40, "도전의성도 5구역" },
        { 41, "투쟁의성도1구역" },
        { 42, "투쟁의성도2구역" },
        { 43, "도전의 성도 6구역" },
        { 44, "투쟁의 성도 5구역" },
        { 45, "Hells Fork" },
        { 46, "Hells Fork 2" },
        { 47, "Bagan 2" },
        { 100, "Casino" },
        { 201, "Deep Cave" },
        { 202, "Nature Cave" },
        { 203, "Dungeon of SungHwa" },
        { 204, "Ancient Tomb Lv. 1" },
        { 205, "Ancient Tomb Lv. 2" },
        { 206, "GM Island 1" },
        { 207, "GM Island 2" },
        { 208, "GM Island 3" },
        { 209, "Graveyard Lv. 1" },
        { 210, "Graveyard Lv. 2" },
        { 300, "12 BF" },
    };

ExportMobs(mobsScr, mobArrFiles, regionFiles, regionTableFiles, mapNames, "mobs.wikitext");
ExportItems(itemsScr, "items.wikitext");
ExportNpcs(npcsScr, itemsScr, questsScr, npcArrFiles, regionFiles, regionTableFiles, mapNames, "npcs.wikitext");
ExportQuests(questsScr, npcsScr, itemsScr, "quests.wikitext");

static string? GetRegionName(
    float x,
    float y,
    RegionFile regionFile,
    RegionTableFile regionTable,
    float gridSize = 256f)
{
    // Convert world coordinates to grid coordinates
    int gridX = (int)((x - regionFile.OriginX) / gridSize);
    int gridY = (int)((y - regionFile.OriginY) / gridSize);

    // Bounds check
    if (gridX < 0 || gridX >= regionFile.Width ||
        gridY < 0 || gridY >= regionFile.Height)
        return null;

    // Flatten index
    int index = gridY * (int)regionFile.Width + gridX;
    byte regionIndex = regionFile.RegionMap[index];

    // Validate index
    if (regionIndex >= regionTable.Entries.Count)
        return null;

    return regionTable.Entries[regionIndex].Name;
}

static string GetLocationName(ICollection<RegionFile> regionFiles, ICollection<RegionTableFile> regionTableFiles, Dictionary<int, string> mapNames, MapId? mapId, float x, float y)
{
    var mapName = mapNames.ContainsKey((int)mapId) ? mapNames[(int)mapId] : $"Map {mapId}";

    // Find the region table for this map
    var regionFile = regionFiles.FirstOrDefault(r => r.MapId == mapId);
    var regionTable = regionTableFiles.FirstOrDefault(r => r.MapId == mapId);
    if (regionFile != null && regionTable != null)
    {
        var regionName = GetRegionName(x, y, regionFile, regionTable);
        if (!string.IsNullOrWhiteSpace(regionName))
        {
            if (regionName.Equals(mapName, StringComparison.CurrentCultureIgnoreCase))
                return mapName; // Avoid redundancy
            return $"{mapName} > {regionName}";
        }
    }
    // No region table found for this map
    return mapName;
}

static void ExportMobs(MobsScrFile2 mobsScr, ICollection<MobArrFile> mobArrFiles, ICollection<RegionFile> regionFiles, ICollection<RegionTableFile> regionTableFiles, Dictionary<int, string> mapNames, string outputPath)
{
    var sb = new StringBuilder();
    sb.AppendLine("== Mobs ==");
    sb.AppendLine("{| class=\"wikitable sortable\"");
    sb.AppendLine("! ID !! Name !! Level !! HP !! Atk !! Base Fame !! Atk% !! Def% !! Coins !! Spawns !! Notes");

    foreach (var mob in mobsScr.Entries.OrderBy(e => e.Lv))
    {
        var atk = mob.MinAtk == mob.MaxAtk ? $"{mob.MaxAtk}" : $"{mob.MinAtk}–{mob.MaxAtk}";
        var spawns = mobArrFiles
            .Where(arr => arr.Entries.Any(e => e.MobId == mob.Id))
            .SelectMany(arr => arr.Entries
                .Where(e => e.MobId == mob.Id)
                .GroupBy(e => new
                {
                    X = Math.Round(e.X, 0),
                    Y = Math.Round(e.Y, 0)
                })
                .Select(g =>
                {
                    var count = g.Count();
                    var coords = $"({(long)g.Key.X}, {(long)g.Key.Y})";
                    var suffix = count > 1 ? $" x{count}" : "";
                    string location = GetLocationName(regionFiles, regionTableFiles, mapNames, arr.MapId, (float)g.Key.X, (float)g.Key.Y);
                    return $"{location} {coords}{suffix}";
                })
            )
            .ToList();
        var spawnsText = string.Join("<br>", spawns);

        sb.AppendLine("|-");
        sb.AppendLine($"| {(int)mob.Id} || {mob.Name} || {mob.Lv} || {mob.HP} || {atk} || {mob.BaseFame} || {mob.AtkSucceed} || {mob.DefSucceed} || {mob.Coins} || {spawnsText} || {mob.Location}");
    }

    sb.AppendLine("|}");
    File.WriteAllText(outputPath, sb.ToString());
}
static void ExportItems(ItemsScrFile itemsScr, string outputPath)
{
    var sb = new StringBuilder();
    sb.AppendLine("== Items ==");
    sb.AppendLine("{| class=\"wikitable sortable\"");
    sb.AppendLine("! ID !! Name !! Base ID !! Base Name !! Category ID");

    foreach (var item in itemsScr.Entries)
    {
        var baseItem = itemsScr.Entries.FirstOrDefault(i => i.Id == item.BaseId);
        var baseName = baseItem?.Name ?? ((int)item.BaseId).ToString();
        sb.AppendLine("|-");
        sb.AppendLine($"| {(int)item.Id} || {item.Name} || {item.BaseId} || {baseName} || {item.CategoryId}");
    }

    sb.AppendLine("|}");
    File.WriteAllText(outputPath, sb.ToString());
}
static void ExportNpcs(NpcsScrFile npcsScr, ItemsScrFile itemsScr, QuestScrFile questScr, ICollection<NpcArrFile> npcArrFiles, ICollection<RegionFile> regionFiles, ICollection<RegionTableFile> regionTableFiles, Dictionary<int, string> mapNames, string outputPath)
{
    var sb = new StringBuilder();
    sb.AppendLine("== NPCs ==");
    sb.AppendLine("{| class=\"wikitable sortable\"");
    sb.AppendLine("! ID !! Name !! Title !! Type !! Inventory !! Gives Quests !! Rewards Quests !! Locations");

    foreach (var npc in npcsScr.Entries)
    {
        var invNames = string.Join("<br>", npc.Inventory
            .Where(x => x.ItemId != 0)
            .Select(x => itemsScr.Entries.FirstOrDefault(i => i.Id == x.ItemId)?.Name ?? x.ItemId.ToString())
            .Distinct());

        var giveQuests = string.Join("<br>", new[] { npc.GiveQuest1, npc.GiveQuest2, npc.GiveQuest3, npc.GiveQuest4, npc.GiveQuest5, npc.GiveQuest6 }
            .Where(q => q != 0)
            .Select(q => questScr.Entries.FirstOrDefault(e => e.Id == q)?.Name ?? $"Removed quest #{q}")
            .Distinct());

        var rewardQuests = string.Join("<br>", new[] { npc.RewardQuest, npc.RewardQuest2 }
            .Where(q => q != 0)
            .Select(q => questScr.Entries.FirstOrDefault(e => e.Id == q)?.Name ?? $"Removed quest #{q}")
            .Distinct());

        var locations = string.Join("<br>", npcArrFiles.Where(arr => arr.Entries.Any(e => e.NpcId == npc.Id))
            .SelectMany(arr => arr.Entries.Where(e => e.NpcId == npc.Id).Select(e =>
            {
                string location = GetLocationName(regionFiles, regionTableFiles, mapNames, arr.MapId, (float)e.X, (float)e.Y);
                return $"{location} ({(long)e.X}, {(long)e.Y})";
            }))
            .ToList());

        sb.AppendLine("|-");
        sb.AppendLine($"| {(int)npc.Id} || {npc.Name} || {npc.Title} || {npc.NpcType} || {invNames} || {giveQuests} || {rewardQuests} || {locations}");
    }

    sb.AppendLine("|}");
    File.WriteAllText(outputPath, sb.ToString());
}
static void ExportQuests(QuestScrFile questsScr, NpcsScrFile npcsScr, ItemsScrFile itemsScr, string outputPath)
{
    var sb = new StringBuilder();
    sb.AppendLine("== Quests ==");
    sb.AppendLine("{| class=\"wikitable sortable\"");
    sb.AppendLine("! ID !! Name !! Level Range !! Quest Giver !! Reward NPC !! Reward Items !! Exp !! Coins !! Fame !! Steps !! Event !! Allowed Classes !! Required Faction");

    foreach (var quest in questsScr.Entries.OrderBy(e => e.MinLv))
    {
        var giverNpcs = npcsScr.Entries
            .Where(n => n.GiveQuest1 == (QuestId)quest.Id ||
                        n.GiveQuest2 == (QuestId)quest.Id ||
                        n.GiveQuest3 == (QuestId)quest.Id ||
                        n.GiveQuest4 == (QuestId)quest.Id ||
                        n.GiveQuest5 == (QuestId)quest.Id ||
                        n.GiveQuest6 == (QuestId)quest.Id)
            .Select(n => n.Name);

        var rewardNpc = npcsScr.Entries.FirstOrDefault(n => n.RewardQuest2 == (QuestId)quest.Id);

        var rewardItems = rewardNpc?.Inventory
            .Where(i => i.ItemId != 0)
            .Select(i => itemsScr.Entries.FirstOrDefault(it => it.Id == i.ItemId)?.Name ?? i.ItemId.ToString())
            .Take(5);

        // Use bullet list inside table cell
        var stepNames = string.Join("<br>", quest.Steps
            .Take((int)quest.StepCount)
            .Select(s => s.Name));

        var classes = new[]
        {
            quest.AllowWarrior != 0 ? "Warrior" : null,
            quest.AllowAssassin != 0 ? "Assassin" : null,
            quest.AllowMage != 0 ? "Mage" : null,
            quest.AllowMonk != 0 ? "Monk" : null
        }.Where(c => c != null).ToArray();
        if (classes.Length == 4)
            classes = new[] { "All" };

        var rewardItemsStr = string.Join(", ", rewardItems ?? Enumerable.Empty<string>());
        var exp = quest.Steps[Math.Max(0, quest.StepCount - 1)].Exp;
        var coin = quest.Steps[Math.Max(0, quest.StepCount - 1)].Gold;
        var fame = quest.Steps[Math.Max(0, quest.StepCount - 1)].Fame;

        sb.AppendLine("|-");
        sb.AppendLine($"| {(int)quest.Id} || {quest.Name} || {quest.MinLv}–{quest.MaxLv} || {string.Join("<br>", giverNpcs)} || {rewardNpc?.Name ?? ""} || {rewardItemsStr} || {exp} || {coin} || {fame} || {stepNames} || {(quest.IsEvent == 1 ? "Yes" : "No")} || {string.Join(", ", classes)} || {quest.RequiredFaction}");
    }

    sb.AppendLine("|}");
    File.WriteAllText(outputPath, sb.ToString());
}