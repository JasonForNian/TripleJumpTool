using BepInEx;

namespace TripleJumpTool;

// TODO - adjust the plugin guid as needed
[BepInAutoPlugin(id: "io.github.jasonfornian.triplejumptool")]
public partial class TripleJumpToolPlugin : BaseUnityPlugin
{
    private void Awake()
    {
        // Put your initialization logic here
        Logger.LogInfo($"Plugin {Name} ({Id}) has loaded!");
    }
}
