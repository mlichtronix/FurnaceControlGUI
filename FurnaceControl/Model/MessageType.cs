namespace FurnaceControl
{
    public enum MessageType
    {
        NoOp              = 100,    // No Operation / Empty message.    
        HandShake         = 200,    // Identify Device by proper Handshake.
        SetTime           = 300,    // Set Time In Furnace.
        GetCurTemp        = 400,    // Get Current Temperature in furnace.
        GetPcStatus       = 500,    // Get Furnace Program Counter Status.
        GetCurProg        = 600,    // Get Current running Program from furnace.
        SetProgram        = 650,    // Load customized program.
        Start             = 700,    // Start selected program.
        CloseSmokeAlert   = 800,    // Furnace reached temperature and need to close smokestack.
        LogMessage        = 900,    // Unspecified Logging message.
        Heating           = 950,    // Heating Update.
        Error             = 990,    // Error message.
        Invalid           = 995,    // Message is mismatch / invalid type.
        Halt              = 999,    // Halt all Furnace operations and cool down.
    }
}
