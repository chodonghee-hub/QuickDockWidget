namespace QuickDock.Models
{
    public class HotkeyConfig
    {
        public uint Modifiers { get; set; } = 0x4002; // MOD_CTRL | MOD_NOREPEAT
        public uint Vk        { get; set; } = 0xC0;   // backtick (`)
    }
}
