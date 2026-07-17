using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using SilkySouls2.Interfaces;
using SilkySouls2.Memory.Patterns;

namespace SilkySouls2.Memory
{
    public class AoBScanner(IMemoryService memoryService)
    {
        
        private const int HistogramSampleStep = 16;

        private byte[]? _module;
        private nint _moduleBase;

        private readonly List<Request> _requests = new();

        private readonly byte[] _bitmap = new byte[65536 / 8];
        private readonly List<Request>?[] _pairBuckets = new List<Request>[65536];

        private readonly List<Request>?[] _singleBuckets = new List<Request>[256];
        private bool _hasSingleFallback;

        private readonly Dictionary<string, nint> _savedAddresses = new();
        private readonly long[] _pairHistogram = new long[65536];
        private long _histogramSamples;
        private bool _histogramBuilt;
        
        private static readonly string ScholarBackupPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SilkySouls2",
            "backup_addresses_scholar.txt"); 
        
        private static readonly string VanillaBackupPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SilkySouls2",
            "backup_addresses_vanilla.txt");

        private static string BackupPath => PatchManager.IsScholar() ? ScholarBackupPath : VanillaBackupPath;
        
        private sealed class Request(int id, string? name, Pattern pattern, Action<nint> setter)
        {
            public int Id { get; } = id;
            public string? Name { get; } = name;
            public Pattern Pattern { get; } = pattern;
            public Action<nint> Setter { get; } = setter;
            public int[] NonWildcardIndices { get; } = BuildNonWildcardIndices(pattern);
            public int AnchorOffset;
            public long AnchorFrequency = -1;
            public bool IsSingle;

            private static int[] BuildNonWildcardIndices(Pattern p)
            {
                var len = p.Bytes.Length;
                var list = new List<int>(len);
                for (var j = 0; j < len; j++)
                    if (IsConcrete(p.Mask, j, len))
                        list.Add(j);
                return list.ToArray();
            }
        }
        
        #region Public Methods
        
        public void QueueFallbackPatterns()
        {
            if (PatchManager.IsScholar())
            {
                QueueScholarFallbackPatterns();
                return;
            }

            QueueVanillaFallbackPatterns();
        }

        private void QueueScholarFallbackPatterns()
        {
            Queue(nameof(Patterns64.GameManagerImp), Patterns64.GameManagerImp, addr => Offsets.GameManagerImp.Base = addr);
            Queue(nameof(Patterns64.KatanaMainApp), Patterns64.KatanaMainApp, addr => Offsets.KatanaMainApp.Base = addr);
            Queue(nameof(Patterns64.HkHardwareInfo), Patterns64.HkHardwareInfo, addr => Offsets.HkHardwareInfo.Base = addr);
            Queue(nameof(Patterns64.MapId), Patterns64.MapId, addr => Offsets.MapId = addr);
            Queue(nameof(Patterns64.BuildTextFieldRetAddr), Patterns64.BuildTextFieldRetAddr, addr => Offsets.BuildTextFieldRetAddr = addr);
            
            
            Queue(nameof(Patterns64.InfiniteStam), Patterns64.InfiniteStam, addr => Offsets.Patches.InfiniteStam = addr);
            Queue(nameof(Patterns64.InfiniteGoods), Patterns64.InfiniteGoods, addr => Offsets.Patches.InfiniteGoods = addr);
            Queue(nameof(Patterns64.InfiniteCasts), Patterns64.InfiniteCasts, addr => Offsets.Patches.InfiniteCasts = addr);
            Queue(nameof(Patterns64.InfiniteDurability), Patterns64.InfiniteDurability, addr => Offsets.Patches.InfiniteDurability = addr);
            Queue(nameof(Patterns64.HideChrModels), Patterns64.HideChrModels, addr => Offsets.Patches.HideChrModels = addr);
            Queue(nameof(Patterns64.HideMap), Patterns64.HideMap, addr => Offsets.Patches.HideMap = addr);
            Queue(nameof(Patterns64.DropRate), Patterns64.DropRate, addr => Offsets.Patches.DropRate = addr);
            Queue(nameof(Patterns64.DisableAi), Patterns64.DisableAi, addr => Offsets.Patches.DisableAi = addr);
            Queue(nameof(Patterns64.Silent), Patterns64.Silent, addr => Offsets.Patches.Silent = addr);
            Queue(nameof(Patterns64.Hidden), Patterns64.Hidden, addr => Offsets.Patches.Hidden = addr);
            Queue(nameof(Patterns64.NegativeLevel), Patterns64.NegativeLevel, addr => Offsets.Patches.NegativeLevel = addr);
            Queue(nameof(Patterns64.NoSoulGain), Patterns64.NoSoulGain, addr => Offsets.Patches.NoSoulGain = addr);
            Queue(nameof(Patterns64.NoHollowing), Patterns64.NoHollowing, addr => Offsets.Patches.NoHollowing = addr);
            Queue(nameof(Patterns64.NoSoulLoss), Patterns64.NoSoulLoss, addr => Offsets.Patches.NoSoulLoss = addr);
            Queue(nameof(Patterns64.SoulMemWrite1), Patterns64.SoulMemWrite1, addr => Offsets.Patches.SoulMemWrite1 = addr);
            Queue(nameof(Patterns64.SoulMemWrite2), Patterns64.SoulMemWrite2, addr => Offsets.Patches.SoulMemWrite2 = addr);
            Queue(nameof(Patterns64.NoHitPatch), Patterns64.NoHitPatch, addr => Offsets.Patches.NoHitPatch = addr);
            Queue(nameof(Patterns64.MenuTransition), Patterns64.MenuTransition, addr => Offsets.Patches.MenuTransition = addr);
            Queue(nameof(Patterns64.DisableRoll), Patterns64.DisableRoll, addr => Offsets.Patches.DisableRoll = addr);
            
            
            Queue(nameof(Patterns64.SetAreaVariable), Patterns64.SetAreaVariable, addr => Offsets.Hooks.SetAreaVariable = addr);
            Queue(nameof(Patterns64.CompareEventRandValueForlorn), Patterns64.CompareEventRandValueForlorn, addr => Offsets.Hooks.CompareEventRandValueForlorn = addr);
            Queue(nameof(Patterns64.CompareEventRandValueElana), Patterns64.CompareEventRandValueElana, addr => Offsets.Hooks.CompareEventRandValueElana = addr);
            Queue(nameof(Patterns64.PlayerNoDamage), Patterns64.PlayerNoDamage, addr => Offsets.Hooks.PlayerNoDamage = addr);
            Queue(nameof(Patterns64.LockedTarget), Patterns64.LockedTarget, addr => Offsets.Hooks.LockedTarget = addr);
            Queue(nameof(Patterns64.CreditSkip), Patterns64.CreditSkip, addr => Offsets.Hooks.CreditSkip = addr);
            Queue(nameof(Patterns64.NumOfDrops), Patterns64.NumOfDrops, addr => Offsets.Hooks.NumOfDrops = addr);
            Queue(nameof(Patterns64.DamageControl), Patterns64.DamageControl, addr => Offsets.Hooks.DamageControl = addr);
            Queue(nameof(Patterns64.TriggersAndSpace), Patterns64.TriggersAndSpace, addr => Offsets.Hooks.TriggersAndSpace = addr);
            Queue(nameof(Patterns64.Ctrl), Patterns64.Ctrl, addr => Offsets.Hooks.Ctrl = addr);
            Queue(nameof(Patterns64.NoClipUpdateCoords), Patterns64.NoClipUpdateCoords, addr => Offsets.Hooks.NoClipUpdateCoords = addr);
            Queue(nameof(Patterns64.KillboxFlagSet), Patterns64.KillboxFlagSet, addr => Offsets.Hooks.KillboxFlagSet = addr);
            Queue(nameof(Patterns64.SetCurrentAct), Patterns64.SetCurrentAct, addr => Offsets.Hooks.SetCurrentAct = addr);
            Queue(nameof(Patterns64.FasterMenu), Patterns64.FasterMenu, addr => Offsets.Hooks.FasterMenu = addr);
            Queue(nameof(Patterns64.InfinitePoise), Patterns64.InfinitePoise, addr => Offsets.Hooks.InfinitePoise = addr);
            Queue(nameof(Patterns64.SetEventWrapper), Patterns64.SetEventWrapper, addr => Offsets.Hooks.SetEventWrapper = addr);
            Queue(nameof(Patterns64.ProcessPhysics), Patterns64.ProcessPhysics, addr => Offsets.Hooks.ProcessPhysics = addr);
            Queue(nameof(Patterns64.DisableTargetAi), Patterns64.DisableTargetAi, addr => Offsets.Hooks.DisableTargetAi = addr);
            Queue(nameof(Patterns64.SetSharedFlag), Patterns64.SetSharedFlag, addr => Offsets.Hooks.SetSharedFlag = addr);
            Queue(nameof(Patterns64.BabyJump), Patterns64.BabyJump, addr => Offsets.Hooks.BabyJump = addr);
            Queue(nameof(Patterns64.EzStateCompareTimer), Patterns64.EzStateCompareTimer, addr => Offsets.Hooks.EzStateCompareTimer = addr);
            Queue(nameof(Patterns64.NoShadedFogClose), Patterns64.NoShadedFogClose, addr => Offsets.Hooks.NoShadedFogClose = addr);
            Queue(nameof(Patterns64.ReduceGameSpeed), Patterns64.ReduceGameSpeed, addr => Offsets.Hooks.ReduceGameSpeed = addr);
            Queue(nameof(Patterns64.LightGutter), Patterns64.LightGutter, addr => Offsets.Hooks.LightGutter = addr);
            Queue(nameof(Patterns64.NoShadedFogFar), Patterns64.NoShadedFogFar, addr => Offsets.Hooks.NoShadedFogFar = addr);
            Queue(nameof(Patterns64.NoShadedFogCam), Patterns64.NoShadedFogCam, addr => Offsets.Hooks.NoShadedFogCam = addr);
            Queue(nameof(Patterns64.GameManUpdate), Patterns64.GameManUpdate, addr => Offsets.Hooks.GameManUpdate = addr);
            Queue(nameof(Patterns64.NewGameDetect), Patterns64.NewGameDetect, addr => Offsets.Hooks.NewGameDetect = addr);
            Queue(nameof(Patterns64.LoadingItemName), Patterns64.LoadingItemName, addr => Offsets.Hooks.LoadingItemName = addr);
            Queue(nameof(Patterns64.PreAiEzState), Patterns64.PreAiEzState, addr => Offsets.Hooks.PreAiEzState = addr);
            Queue(nameof(Patterns64.NoShadedFogCamFilter), Patterns64.NoShadedFogCamFilter, addr => Offsets.Hooks.NoShadedFogCamFilter = addr);
            
            
            Queue(nameof(Patterns64.RequestWarp), Patterns64.RequestWarp, addr => Offsets.Functions.RequestWarp = addr);
            Queue(nameof(Patterns64.SetEvent), Patterns64.SetEvent, addr => Offsets.Functions.SetEvent = addr);
            Queue(nameof(Patterns64.GetEvent), Patterns64.GetEvent, addr => Offsets.Functions.GetEvent = addr);
            Queue(nameof(Patterns64.GiveSouls), Patterns64.GiveSouls, addr => Offsets.Functions.GiveSouls = addr);
            Queue(nameof(Patterns64.RestoreSpellcasts), Patterns64.RestoreSpellcasts, addr => Offsets.Functions.RestoreSpellcasts = addr);
            Queue(nameof(Patterns64.ParamLookup), Patterns64.ParamLookup, addr => Offsets.Functions.ParamLookup = addr);
            Queue(nameof(Patterns64.SetRenderTargets), Patterns64.SetRenderTargets, addr => Offsets.Functions.SetRenderTargets = addr);
            Queue(nameof(Patterns64.CreateSoundEvent), Patterns64.CreateSoundEvent, addr => Offsets.Functions.CreateSoundEvent = addr);
            Queue(nameof(Patterns64.LevelLookup), Patterns64.LevelLookup, addr => Offsets.Functions.LevelLookup = addr);
            Queue(nameof(Patterns64.LevelUp), Patterns64.LevelUp, addr => Offsets.Functions.LevelUp = addr);
            Queue(nameof(Patterns64.CurrentItemQuantityCheck), Patterns64.CurrentItemQuantityCheck, addr => Offsets.Functions.CurrentItemQuantityCheck = addr);
            Queue(nameof(Patterns64.ItemGive), Patterns64.ItemGive, addr => Offsets.Functions.ItemGive = addr);
            Queue(nameof(Patterns64.BuildItemDialog), Patterns64.BuildItemDialog, addr => Offsets.Functions.BuildItemDialog = addr);
            Queue(nameof(Patterns64.ShowItemDialog), Patterns64.ShowItemDialog, addr => Offsets.Functions.ShowItemDialog = addr);
            Queue(nameof(Patterns64.GetEyePosition), Patterns64.GetEyePosition, addr => Offsets.Functions.GetEyePosition = addr);
            Queue(nameof(Patterns64.ApplySpEffect), Patterns64.ApplySpEffect, addr => Offsets.Functions.ApplySpEffect = addr);
            Queue(nameof(Patterns64.HavokRayCast), Patterns64.HavokRayCast, addr => Offsets.Functions.HavokRayCast = addr);
            Queue(nameof(Patterns64.ConvertPxRigidToMapEntity), Patterns64.ConvertPxRigidToMapEntity, addr => Offsets.Functions.ConvertPxRigidToMapEntity = addr);
            Queue(nameof(Patterns64.PackGameEntityHandle), Patterns64.PackGameEntityHandle, addr => Offsets.Functions.PackGameEntityHandle = addr);
            Queue(nameof(Patterns64.UnlockBonfire), Patterns64.UnlockBonfire, addr => Offsets.Functions.UnlockBonfire = addr);
            Queue(nameof(Patterns64.GetMapObjStateActComponent), Patterns64.GetMapObjStateActComponent, addr => Offsets.Functions.GetMapObjStateActComponent = addr);
            Queue(nameof(Patterns64.GetMapEntityWithAreaIdAndObjId), Patterns64.GetMapEntityWithAreaIdAndObjId, addr => Offsets.Functions.GetMapEntityWithAreaIdAndObjId = addr);
            Queue(nameof(Patterns64.AttuneSpell), Patterns64.AttuneSpell, addr => Offsets.Functions.AttuneSpell = addr);
            Queue(nameof(Patterns64.GetNumOfSpellSlots1), Patterns64.GetNumOfSpellSlots1, addr => Offsets.Functions.GetNumOfSpellSlots1 = addr);
            Queue(nameof(Patterns64.GetNumOfSpellSlots2), Patterns64.GetNumOfSpellSlots2, addr => Offsets.Functions.GetNumOfSpellSlots2 = addr);
            Queue(nameof(Patterns64.UpdateSpellSlots), Patterns64.UpdateSpellSlots, addr => Offsets.Functions.UpdateSpellSlots = addr);
            Queue(nameof(Patterns64.EzStateExternalEventCtor), Patterns64.EzStateExternalEventCtor, addr => Offsets.Functions.EzStateExternalEventCtor = addr);
            Queue(nameof(Patterns64.EzStateEventExecuteCommand), Patterns64.EzStateEventExecuteCommand, addr => Offsets.Functions.EzStateEventExecuteCommand = addr);
            Queue(nameof(Patterns64.OriginalMakeSound), Patterns64.OriginalMakeSound, addr => Offsets.Functions.OriginalMakeSound = addr);
            Queue(nameof(Patterns64.OriginalSoulGain), Patterns64.OriginalSoulGain, addr => Offsets.Functions.OriginalSoulGain = addr);
            Queue(nameof(Patterns64.OpenNpcMenu), Patterns64.OpenNpcMenu, addr => Offsets.Functions.OpenNpcMenu = addr);
            Queue(nameof(Patterns64.SetMenuOpenChrState), Patterns64.SetMenuOpenChrState, addr => Offsets.Functions.SetMenuOpenChrState = addr);
            Queue(nameof(Patterns64.ApplyDurabilityDamage), Patterns64.ApplyDurabilityDamage, addr => Offsets.Functions.ApplyDurabilityDamage = addr);
            Queue(nameof(Patterns64.ResolveTargetCtrlFromHandle), Patterns64.ResolveTargetCtrlFromHandle, addr => Offsets.Functions.ResolveTargetCtrlFromHandle = addr);
          
   
        }

        private void QueueVanillaFallbackPatterns()
        {
            Queue(nameof(Patterns32.GameManagerImp), Patterns32.GameManagerImp, addr => Offsets.GameManagerImp.Base = addr);
            Queue(nameof(Patterns32.KatanaMainApp), Patterns32.KatanaMainApp, addr => Offsets.KatanaMainApp.Base = addr);
            Queue(nameof(Patterns32.MapId), Patterns32.MapId, addr => Offsets.MapId = addr);
            Queue(nameof(Patterns32.LoadLibraryW), Patterns32.LoadLibraryW, addr => Offsets.LoadLibraryW = addr);
            Queue(nameof(Patterns32.BuildTextFieldRetAddr), Patterns32.BuildTextFieldRetAddr, addr => Offsets.BuildTextFieldRetAddr = addr);

            
            Queue(nameof(Patterns32.InfiniteStam), Patterns32.InfiniteStam, addr => Offsets.Patches.InfiniteStam = addr);
            Queue(nameof(Patterns32.InfiniteGoods), Patterns32.InfiniteGoods, addr => Offsets.Patches.InfiniteGoods = addr);
            Queue(nameof(Patterns32.InfiniteCasts), Patterns32.InfiniteCasts, addr => Offsets.Patches.InfiniteCasts = addr);
            Queue(nameof(Patterns32.InfiniteDurability), Patterns32.InfiniteDurability, addr => Offsets.Patches.InfiniteDurability = addr);
            Queue(nameof(Patterns32.HideChrModels), Patterns32.HideChrModels, addr => Offsets.Patches.HideChrModels = addr);
            Queue(nameof(Patterns32.HideMap), Patterns32.HideMap, addr => Offsets.Patches.HideMap = addr);
            Queue(nameof(Patterns32.DropRate), Patterns32.DropRate, addr => Offsets.Patches.DropRate = addr);
            Queue(nameof(Patterns32.DisableAi), Patterns32.DisableAi, addr => Offsets.Patches.DisableAi = addr);
            Queue(nameof(Patterns32.Silent), Patterns32.Silent, addr => Offsets.Patches.Silent = addr);
            Queue(nameof(Patterns32.Hidden), Patterns32.Hidden, addr => Offsets.Patches.Hidden = addr);
            Queue(nameof(Patterns32.NegativeLevel), Patterns32.NegativeLevel, addr => Offsets.Patches.NegativeLevel = addr);
            Queue(nameof(Patterns32.NoSoulGain), Patterns32.NoSoulGain, addr => Offsets.Patches.NoSoulGain = addr);
            Queue(nameof(Patterns32.NoHollowing), Patterns32.NoHollowing, addr => Offsets.Patches.NoHollowing = addr);
            Queue(nameof(Patterns32.NoSoulLoss), Patterns32.NoSoulLoss, addr => Offsets.Patches.NoSoulLoss = addr);
            Queue(nameof(Patterns32.SoulMemWrite1), Patterns32.SoulMemWrite1, addr => Offsets.Patches.SoulMemWrite1 = addr);
            Queue(nameof(Patterns32.SoulMemWrite2), Patterns32.SoulMemWrite2, addr => Offsets.Patches.SoulMemWrite2 = addr);
            Queue(nameof(Patterns32.NoHitPatch), Patterns32.NoHitPatch, addr => Offsets.Patches.NoHitPatch = addr);
            Queue(nameof(Patterns32.MenuTransition), Patterns32.MenuTransition, addr => Offsets.Patches.MenuTransition = addr);
            Queue(nameof(Patterns32.DisableRoll), Patterns32.DisableRoll, addr => Offsets.Patches.DisableRoll = addr);
            

            Queue(nameof(Patterns32.CompareEventRandValueElana), Patterns32.CompareEventRandValueElana, addr => Offsets.Hooks.CompareEventRandValueElana = addr);
            Queue(nameof(Patterns32.PlayerNoDamage), Patterns32.PlayerNoDamage, addr => Offsets.Hooks.PlayerNoDamage = addr);
            Queue(nameof(Patterns32.LockedTarget), Patterns32.LockedTarget, addr => Offsets.Hooks.LockedTarget = addr);
            Queue(nameof(Patterns32.CreditSkip), Patterns32.CreditSkip, addr => Offsets.Hooks.CreditSkip = addr);
            Queue(nameof(Patterns32.NumOfDrops), Patterns32.NumOfDrops, addr => Offsets.Hooks.NumOfDrops = addr);
            Queue(nameof(Patterns32.DamageControl), Patterns32.DamageControl, addr => Offsets.Hooks.DamageControl = addr);
            Queue(nameof(Patterns32.TriggersAndSpace), Patterns32.TriggersAndSpace, addr => Offsets.Hooks.TriggersAndSpace = addr);
            Queue(nameof(Patterns32.Ctrl), Patterns32.Ctrl, addr => Offsets.Hooks.Ctrl = addr);
            Queue(nameof(Patterns32.NoClipUpdateCoords), Patterns32.NoClipUpdateCoords, addr => Offsets.Hooks.NoClipUpdateCoords = addr);
            Queue(nameof(Patterns32.KillboxFlagSet), Patterns32.KillboxFlagSet, addr => Offsets.Hooks.KillboxFlagSet = addr);
            Queue(nameof(Patterns32.SetCurrentAct), Patterns32.SetCurrentAct, addr => Offsets.Hooks.SetCurrentAct = addr);
            Queue(nameof(Patterns32.FasterMenu), Patterns32.FasterMenu, addr => Offsets.Hooks.FasterMenu = addr);
            Queue(nameof(Patterns32.InfinitePoise), Patterns32.InfinitePoise, addr => Offsets.Hooks.InfinitePoise = addr);
            Queue(nameof(Patterns32.SetEventWrapper), Patterns32.SetEventWrapper, addr => Offsets.Hooks.SetEventWrapper = addr);
            Queue(nameof(Patterns32.ProcessPhysics), Patterns32.ProcessPhysics, addr => Offsets.Hooks.ProcessPhysics = addr);
            Queue(nameof(Patterns32.DisableTargetAi), Patterns32.DisableTargetAi, addr => Offsets.Hooks.DisableTargetAi = addr);
            Queue(nameof(Patterns32.SetSharedFlag), Patterns32.SetSharedFlag, addr => Offsets.Hooks.SetSharedFlag = addr);
            Queue(nameof(Patterns32.BabyJump), Patterns32.BabyJump, addr => Offsets.Hooks.BabyJump = addr);
            Queue(nameof(Patterns32.EzStateCompareTimer), Patterns32.EzStateCompareTimer, addr => Offsets.Hooks.EzStateCompareTimer = addr);
            Queue(nameof(Patterns32.NoShadedFogClose), Patterns32.NoShadedFogClose, addr => Offsets.Hooks.NoShadedFogClose = addr);
            Queue(nameof(Patterns32.ReduceGameSpeed), Patterns32.ReduceGameSpeed, addr => Offsets.Hooks.ReduceGameSpeed = addr);
            Queue(nameof(Patterns32.LightGutter), Patterns32.LightGutter, addr => Offsets.Hooks.LightGutter = addr);
            Queue(nameof(Patterns32.NoShadedFogFar), Patterns32.NoShadedFogFar, addr => Offsets.Hooks.NoShadedFogFar = addr);
            Queue(nameof(Patterns32.NoShadedFogCam), Patterns32.NoShadedFogCam, addr => Offsets.Hooks.NoShadedFogCam = addr);
            Queue(nameof(Patterns32.NewGameDetect), Patterns32.NewGameDetect, addr => Offsets.Hooks.NewGameDetect = addr);
            Queue(nameof(Patterns32.LoadingItemName), Patterns32.LoadingItemName, addr => Offsets.Hooks.LoadingItemName = addr);
            Queue(nameof(Patterns32.PreAiEzState), Patterns32.PreAiEzState, addr => Offsets.Hooks.PreAiEzState = addr);
            
            
            Queue(nameof(Patterns32.RequestWarp), Patterns32.RequestWarp, addr => Offsets.Functions.RequestWarp = addr);
            Queue(nameof(Patterns32.SetEvent), Patterns32.SetEvent, addr => Offsets.Functions.SetEvent = addr);
            Queue(nameof(Patterns32.GetEvent), Patterns32.GetEvent, addr => Offsets.Functions.GetEvent = addr);
            Queue(nameof(Patterns32.GiveSouls), Patterns32.GiveSouls, addr => Offsets.Functions.GiveSouls = addr);
            Queue(nameof(Patterns32.RestoreSpellcasts), Patterns32.RestoreSpellcasts, addr => Offsets.Functions.RestoreSpellcasts = addr);
            Queue(nameof(Patterns32.ParamLookup), Patterns32.ParamLookup, addr => Offsets.Functions.ParamLookup = addr);
            Queue(nameof(Patterns32.CreateSoundEvent), Patterns32.CreateSoundEvent, addr => Offsets.Functions.CreateSoundEvent = addr);
            Queue(nameof(Patterns32.LevelLookup), Patterns32.LevelLookup, addr => Offsets.Functions.LevelLookup = addr);
            Queue(nameof(Patterns32.LevelUp), Patterns32.LevelUp, addr => Offsets.Functions.LevelUp = addr);
            Queue(nameof(Patterns32.CurrentItemQuantityCheck), Patterns32.CurrentItemQuantityCheck, addr => Offsets.Functions.CurrentItemQuantityCheck = addr);
            Queue(nameof(Patterns32.ItemGive), Patterns32.ItemGive, addr => Offsets.Functions.ItemGive = addr);
            Queue(nameof(Patterns32.BuildItemDialog), Patterns32.BuildItemDialog, addr => Offsets.Functions.BuildItemDialog = addr);
            Queue(nameof(Patterns32.ShowItemDialog), Patterns32.ShowItemDialog, addr => Offsets.Functions.ShowItemDialog = addr);
            Queue(nameof(Patterns32.GetEyePosition), Patterns32.GetEyePosition, addr => Offsets.Functions.GetEyePosition = addr);
            Queue(nameof(Patterns32.ApplySpEffect), Patterns32.ApplySpEffect, addr => Offsets.Functions.ApplySpEffect = addr);
            Queue(nameof(Patterns32.HavokRayCast), Patterns32.HavokRayCast, addr => Offsets.Functions.HavokRayCast = addr);
            Queue(nameof(Patterns32.ConvertPxRigidToMapEntity), Patterns32.ConvertPxRigidToMapEntity, addr => Offsets.Functions.ConvertPxRigidToMapEntity = addr);
            Queue(nameof(Patterns32.PackGameEntityHandle), Patterns32.PackGameEntityHandle, addr => Offsets.Functions.PackGameEntityHandle = addr);
            Queue(nameof(Patterns32.UnlockBonfire), Patterns32.UnlockBonfire, addr => Offsets.Functions.UnlockBonfire = addr);
            Queue(nameof(Patterns32.GetMapObjStateActComponent), Patterns32.GetMapObjStateActComponent, addr => Offsets.Functions.GetMapObjStateActComponent = addr);
            Queue(nameof(Patterns32.GetMapEntityWithAreaIdAndObjId), Patterns32.GetMapEntityWithAreaIdAndObjId, addr => Offsets.Functions.GetMapEntityWithAreaIdAndObjId = addr);
            Queue(nameof(Patterns32.AttuneSpell), Patterns32.AttuneSpell, addr => Offsets.Functions.AttuneSpell = addr);
            Queue(nameof(Patterns32.GetNumOfSpellSlots1), Patterns32.GetNumOfSpellSlots1, addr => Offsets.Functions.GetNumOfSpellSlots1 = addr);
            Queue(nameof(Patterns32.GetNumOfSpellSlots2), Patterns32.GetNumOfSpellSlots2, addr => Offsets.Functions.GetNumOfSpellSlots2 = addr);
            Queue(nameof(Patterns32.UpdateSpellSlots), Patterns32.UpdateSpellSlots, addr => Offsets.Functions.UpdateSpellSlots = addr);
            Queue(nameof(Patterns32.Sleep), Patterns32.Sleep, addr => Offsets.Functions.Sleep = addr);
            Queue(nameof(Patterns32.SetDepthStencilSurface), Patterns32.SetDepthStencilSurface, addr => Offsets.Functions.SetDepthStencilSurface = addr);
            Queue(nameof(Patterns32.EzStateExternalEventCtor), Patterns32.EzStateExternalEventCtor, addr => Offsets.Functions.EzStateExternalEventCtor = addr);
            Queue(nameof(Patterns32.EzStateEventExecuteCommand), Patterns32.EzStateEventExecuteCommand, addr => Offsets.Functions.EzStateEventExecuteCommand = addr);
            Queue(nameof(Patterns32.OriginalMakeSound), Patterns32.OriginalMakeSound, addr => Offsets.Functions.OriginalMakeSound = addr);
            Queue(nameof(Patterns32.OriginalSoulGain), Patterns32.OriginalSoulGain, addr => Offsets.Functions.OriginalSoulGain = addr);
            Queue(nameof(Patterns32.OpenNpcMenu), Patterns32.OpenNpcMenu, addr => Offsets.Functions.OpenNpcMenu = addr);
            Queue(nameof(Patterns32.SetMenuOpenChrState), Patterns32.SetMenuOpenChrState, addr => Offsets.Functions.SetMenuOpenChrState = addr);
            Queue(nameof(Patterns32.ApplyDurabilityDamage), Patterns32.ApplyDurabilityDamage, addr => Offsets.Functions.ApplyDurabilityDamage = addr);
            Queue(nameof(Patterns32.ResolveTargetCtrlFromHandle), Patterns32.ResolveTargetCtrlFromHandle, addr => Offsets.Functions.ResolveTargetCtrlFromHandle = addr);
        }
        
        public void Run()
        {
            if (_module is null) LoadModule();
            LoadSavedAddresses();
            AssignAnchors();

#if DEBUG
            LogAnchors();
            var scan = Stopwatch.StartNew();
#endif
            var buf = _module!;
            var bufLen = buf.Length;
            var end = bufLen - 1; 
            ref var bufRef = ref buf[0];

            var found = new bool[_requests.Count];
            var matchCounts = new int[_requests.Count];
            var remaining = _requests.Count;

            for (var i = 0; i < end && remaining > 0; i++)
            {
                var b0 = Unsafe.Add(ref bufRef, i);
                var key = b0 | (Unsafe.Add(ref bufRef, i + 1) << 8);

                if ((_bitmap[key >> 3] & (1 << (key & 7))) != 0)
                {
                    var bucket = _pairBuckets[key];
                    if (bucket != null)
                    {
                        foreach (var req in bucket)
                        {
                            if (found[req.Id]) continue;
                            var start = i - req.AnchorOffset;
                            if (start < 0) continue;
                            if (!Matches(ref bufRef, bufLen, start, req)) continue;
                            if (!AcceptOccurrence(req, matchCounts)) continue;
                            ResolveAndInvoke(req, start);
                            found[req.Id] = true;
                            remaining--;
                        }
                    }
                }

                if (!_hasSingleFallback) continue;
                {
                    var sb = _singleBuckets[b0];
                    if (sb == null) continue;
                    foreach (var req in sb)
                    {
                        if (found[req.Id]) continue;
                        var start = i - req.AnchorOffset;
                        if (start < 0) continue;
                        if (!Matches(ref bufRef, bufLen, start, req)) continue;
                        if (!AcceptOccurrence(req, matchCounts)) continue;
                        ResolveAndInvoke(req, start);
                        found[req.Id] = true;
                        remaining--;
                    }
                }
            }

            for (var i = 0; i < _requests.Count; i++)
            {
                if (found[i]) continue;
                var req = _requests[i];
                if (req.Name != null && _savedAddresses.TryGetValue(req.Name, out var saved))
                {
                    req.Setter(saved);
#if DEBUG
                    Console.WriteLine($"[AobScanner] MISS (using saved): {req.Name}");
#endif
                }
                else
                {
                    req.Setter(0);
#if DEBUG
                    Console.WriteLine($"[AobScanner] MISS (no saved): {req.Name}");
#endif
                }
            }

            WriteSavedAddresses();

#if DEBUG
            scan.Stop();
            var foundCount = _requests.Count - remaining;
            Console.WriteLine(
                $"[AobScanner] scan done in {scan.ElapsedMilliseconds} ms ({foundCount}/{_requests.Count} found)");
#endif
        }
        
        #endregion
        
        #region Private Methods

        private static bool IsConcrete(string mask, int j, int len)
            => j < len && (j >= mask.Length || mask[j] != '?');
        
        private void LoadModule()
        {
            _moduleBase = memoryService.BaseAddress;
            _module = memoryService.ReadBytes(_moduleBase, memoryService.ModuleMemorySize);
        }
        
        private void Queue(string? name, Pattern pattern, Action<nint> setter) =>
            _requests.Add(new Request(_requests.Count, name, pattern, setter));
        
        private void LoadSavedAddresses()
        {
            _savedAddresses.Clear();
            if (!File.Exists(BackupPath)) return;
            foreach (var line in File.ReadAllLines(BackupPath))
            {
                var parts = line.Split('=');
                if (parts.Length != 2) continue;
                if (long.TryParse(parts[1], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var val))
                    _savedAddresses[parts[0]] = (nint)val;
            }
        }

        private void WriteSavedAddresses()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(BackupPath)!);
            using var writer = new StreamWriter(BackupPath);
            foreach (var kvp in _savedAddresses)
                writer.WriteLine($"{kvp.Key}={(long)kvp.Value:X}");
        }

        private void BuildHistogram()
        {
            Array.Clear(_pairHistogram, 0, _pairHistogram.Length);
            var buf = _module!;
            var end = buf.Length - 1;
            long samples = 0;
            for (var i = 0; i < end; i += HistogramSampleStep)
            {
                var key = buf[i] | (buf[i + 1] << 8);
                _pairHistogram[key]++;
                samples++;
            }

            _histogramSamples = samples;
            _histogramBuilt = true;
        }
        
        private void AssignAnchors()
        {
            var needHistogram = _requests.Any(r => r.Pattern.AnchorOffset < 0);
#if DEBUG
            needHistogram = true;
#endif
            if (needHistogram) BuildHistogram();

            long[]? singleMarginal = null; 

            foreach (var req in _requests)
            {
                var bytes = req.Pattern.Bytes;
                var hardOffset = req.Pattern.AnchorOffset;

                if (hardOffset >= 0 && hardOffset + 1 < bytes.Length)
                {
                    AssignPair(req, hardOffset);
                    continue;
                }
                
                var mask = req.Pattern.Mask;
                var len = bytes.Length;
                var bestOffset = -1;
                var bestFreq = long.MaxValue;
                for (var j = 0; j + 1 < len; j++)
                {
                    if (!IsConcrete(mask, j, len) || !IsConcrete(mask, j + 1, len)) continue;
                    var freq = _pairHistogram[bytes[j] | (bytes[j + 1] << 8)];
                    if (freq < bestFreq)
                    {
                        bestFreq = freq;
                        bestOffset = j;
                    }
                }

                if (bestOffset >= 0)
                {
                    AssignPair(req, bestOffset);
                }
                else
                {
                    singleMarginal ??= BuildSingleByteMarginal();
                    var bestByteOffset = req.NonWildcardIndices.Length > 0 ? req.NonWildcardIndices[0] : 0;
                    var bestByteFreq = long.MaxValue;
                    foreach (var j in req.NonWildcardIndices)
                    {
                        var freq = singleMarginal[bytes[j]];
                        if (freq < bestByteFreq)
                        {
                            bestByteFreq = freq;
                            bestByteOffset = j;
                        }
                    }

                    req.IsSingle = true;
                    req.AnchorOffset = bestByteOffset;
                    req.AnchorFrequency = bestByteFreq;
                    _hasSingleFallback = true;
                    (_singleBuckets[bytes[bestByteOffset]] ??= new List<Request>()).Add(req);
                }
            }
        }

        private void AssignPair(Request req, int offset)
        {
            var bytes = req.Pattern.Bytes;
            var key = bytes[offset] | (bytes[offset + 1] << 8);
            req.AnchorOffset = offset;
            req.IsSingle = false;
            req.AnchorFrequency = _histogramBuilt ? _pairHistogram[key] : -1;
            _bitmap[key >> 3] |= (byte)(1 << (key & 7));
            (_pairBuckets[key] ??= new List<Request>()).Add(req);
        }
        
        private long[] BuildSingleByteMarginal()
        {
            var marginal = new long[256];
            for (var key = 0; key < _pairHistogram.Length; key++)
                marginal[key & 0xFF] += _pairHistogram[key];
            return marginal;
        }
        
        private static bool Matches(ref byte bufRef, int bufLen, int start, Request req)
        {
            var bytes = req.Pattern.Bytes;
            var indices = req.NonWildcardIndices;
            if (start + bytes.Length > bufLen) return false;

            foreach (var j in indices)
            {
                if (Unsafe.Add(ref bufRef, start + j) != bytes[j]) return false;
            }

            return true;
        }

        private static bool AcceptOccurrence(Request req, int[] matchCounts)
            => matchCounts[req.Id]++ >= req.Pattern.OccurrenceIndex;

        private void ResolveAndInvoke(Request req, int startIndex)
        {
            var p = req.Pattern;
            var instructionAddress = _moduleBase + startIndex + p.InstructionOffset;

            var final = p.AddressingMode switch
            {
                AddressingMode.Absolute => instructionAddress,
                AddressingMode.Direct32 => (nint)(uint)ReadInt32(instructionAddress + p.OffsetLocation),
                _ => instructionAddress + ReadInt32(instructionAddress + p.OffsetLocation) + p.InstructionLength
            };

            if (req.Name != null) _savedAddresses[req.Name] = final;
            req.Setter(final);
        }

        private int ReadInt32(nint address)
        {
            var idx = (int)(address - _moduleBase);
            return Unsafe.ReadUnaligned<int>(ref _module![idx]);
        }
        
        #endregion

#if DEBUG
        private void LogAnchors()
        {
            double scale = HistogramSampleStep;
            long totalCandidateEst = 0;

            Console.WriteLine($"[AobScanner] --- anchor report ({_requests.Count} patterns, " +
                              $"{_requests.Count(r => r.IsSingle)} single-fallback) ---");
            Console.WriteLine("[AobScanner]   freq(ppm)  count~   combo      off  name");

            foreach (var req in _requests.OrderByDescending(r => r.AnchorFrequency))
            {
                var estCount = (long)(req.AnchorFrequency * scale);
                var ppm = _histogramSamples > 0 ? req.AnchorFrequency / (double)_histogramSamples * 1_000_000 : 0;
                if (req.AnchorFrequency >= 0) totalCandidateEst += estCount;

                if (req.IsSingle)
                {
                    var b = req.Pattern.Bytes[req.AnchorOffset];
                    Console.WriteLine(
                        $"[AobScanner]   {ppm,8:F1}  {estCount,8}  0x{b:X2}(1byte) {req.AnchorOffset,3}   {req.Name}  <-- SINGLE-BYTE FALLBACK");
                    continue;
                }

                var b0 = req.Pattern.Bytes[req.AnchorOffset];
                var b1 = req.Pattern.Bytes[req.AnchorOffset + 1];
                Console.WriteLine(
                    $"[AobScanner]   {ppm,8:F1}  {estCount,8}  0x{b0:X2} 0x{b1:X2}  {req.AnchorOffset,3}   {req.Name}");
            }
        }
    }
#endif
}
