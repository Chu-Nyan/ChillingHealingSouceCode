public enum MapCategory
{
    Outside,
    Inside
}

public enum MethodResult
{
    Failure,
    Running,
    Success
}

public enum MapType
{
    Forest,
    Sea,
    Car
}

public enum ItemCategory
{
    Equipment,
    Consumable,
    Furniture,
    Resources
}

public enum UseEffectType
{
    HP = 1000,
    CleanLiness,
    GetHealingCoin,
    FourLeafCloverConversion,
    Upgrade,
    RandomItem,
    InappPackage,
    AdSkip,
    RandomHealingCoin2,
    RandomHealingCoin,
    ExpandInventory,
    RareRandomItem,
    MapResources = 3000,
    MapPollution = 4000,
    None = 9999,
}

public enum UnitType
{
    FireCow,
    MochiCow,
    BlueGo,
    MelodyGo,
    NolTo,
    RestSheep,
    MoSheep,
    BaitChicken,
    MelodyPig,
    GoodNight,
    None = 9999,
}

public enum CampingType
{
    Bonfire = 1000,
    Table,
    FruitTree,
    Guitar,
    Picnic,
    WatchWater,
    Shellfish,
    Fishing,
    Guitar2,
    Nap,
    Bed,
    Shower,
    Eat,
    MissionBoard = 2000,
    ATM = 2001,
    Clover = 3000,
    Teleport = 4000
}

public enum InteractionType
{
    Running,
    Once
}

public enum StatusType
{
    HP,
    Cleanliness,
    HealingCoin,
    MissionCoin
}

public enum CoinType
{
    Healing,
    Mission,
}

public enum UpgradeType
{
    Camping,
    NPC
}

public enum SoundType
{
    None = 0,
    Potato = 10000,
    WatchingFire1,
    WatchingFire2,
    WatchingFire3,
    WatchingFire4,
    WatchingFire5,
    DrinkingTea,
    PickingFruits,
    PlayingGuitar,
    Picnic,
    WaterGazing1,
    WaterGazing2,
    WaterGazing3,
    WaterGazing4,
    WaterGazing5,
    WaterGazing6,
    WavesAndSeagulls,
    WalkingOnSand1,
    WalkingOnSand2,
    WalkingInShallowWater,
    WaitingWhileFishing,
    GentleBreathing,
    PlayingGuitar2,
    Cooking,
    TakingAShower,
    SeasideVillageAmbience,

    TeacupClink1 = 20000,
    TeacupClink2,
    PouringTea,
    RustlingLeaves1,
    RustlingLeaves2,
    RustlingLeaves3,
    GuitarSetup,
    SeagullCry,
    PickingUp1,
    PickingUp2,
    CoinDrop,
    DoorOpening,
    ItemBoxOpening,
    ButtonPress,
    HealthHygienePenaltyPopup,
    CloverAcquiredPopup,
    PlayerFootsteps,
    InteriorPlacement,
    Upgrade,
    Bang,
    CharacterOutfitSave,
    CharacterOutfitEquip,
    CharacterOutfitUnequip,
    InteriorTrashcan,
    
    SunnyDay = 30000,
}

public enum PackageType
{
    start,
    condition,
    ad_remove,
    area_open,
    hair_v1,
    hair_v2,
    dress_v1,
    dress_v2,
    furniture_v1,
    furniture_v2,
    accessary,
    deco_v1,
    deco_v2,
    deco_v3,
    healling_random_v1,
    healling_random_v2,
    coin1,
    coin2,
    coin3,
    coin4,
    coin5,
    coin6,
    monthly_fee,
}

public enum DialogType
{
    InActiveCamping,
    OutActiveCamping,
    GiveCoin,
    SoloActiveCamping,
    ActiveHealing
}