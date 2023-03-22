namespace SWPMapParser.Destination
{
    internal class TileEntity
    {
        public TileEntryType Type { get; set; }
        public Orientation Orientation { get; set; }
    }

    internal enum Orientation
    {
        NORTH,
        EAST,
        SOUTH,
        WEST
    }
    

    internal enum TileEntryType
    {
        Doppellaser,
        Dreifachlaser,
        ExpressbandKurveLinks,
        ExpressbandKurveRechts,
        ExpressbandGeradeaus,
        ExpressbandGeradeausMitEingangLinks,
        ExpressbandGeradeausMitEingangRechts,
        ExpressbandEingangLinksUndRechts,
        FließbandKurveLinks,
        FließbandKurveRechts,
        FließbandGeradeaus,
        FließbandGeradeausMitEingangLinks,
        FließbandGeradeausMitEingangRechts,
        FließbandEingangLinksUndRechts,
        CrusherGeradeausMitEingangLinksPhase2und4,
        CrusherGeradeausMitEingangLinksPhase3,
        CrusherGeradeausPhase1und5,
        CrusherGeradeausPhase4und2,
        DrehfeldLinksrum,
        DrehfeldRechtsrum,
        Doppelreparatur,
        BodenDefault,
        Laser,
        LaserBeam, //VERYYYY IMPORTANT!!! These tile entities have to be deleted, was just for map creation
        Grube,
        Reparatur,
        SchieberPhase1,
        SchieberPhase2,
        SchieberPhase3,
        SchieberPhase2und4,
        SchieberPhase1und3und5,
        Wand,
        DoubleLaserBeam, //VERYYYY IMPORTANT!!! These tile entities have to be deleted, was just for map creation
        TripleLaserBeam //VERYYYY IMPORTANT!!! These tile entities have to be deleted, was just for map creation
    }
}
