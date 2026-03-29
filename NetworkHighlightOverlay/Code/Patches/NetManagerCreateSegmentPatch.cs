using System.Reflection;
using HarmonyLib;
using NetworkHighlightOverlay.Core;

namespace NetworkHighlightOverlay.Patches
{
    [HarmonyPatch]
    public static class NetManagerCreateSegmentPatch
    {
        static MethodBase TargetMethod() => PatchTargets.ResolveCreateSegment();
        
        static void Postfix(ref ushort segment, NetInfo info, bool __result)
        {
            RuntimeHooks.HandleSegmentCreated(segment, info, __result);
        }
    }
}
