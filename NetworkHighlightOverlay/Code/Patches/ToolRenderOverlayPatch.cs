using HarmonyLib;
using NetworkHighlightOverlay.Core;

namespace NetworkHighlightOverlay.Patches
{
    [HarmonyPatch(typeof(ToolBase), "RenderOverlay")]
    public static class ToolRenderOverlayPatch
    {
        static void Postfix(RenderManager.CameraInfo cameraInfo)
        {
            RuntimeHooks.RenderOverlay(cameraInfo);
        }
    }
}
