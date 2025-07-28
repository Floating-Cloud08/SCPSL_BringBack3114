using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.API.Interfaces;
using Exiled.Events.EventArgs.Player;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace BringBack3114
{
    public class PluginConfig : IConfig
    {
        [Description("启用")]
        public bool IsEnabled { get; set; } = true;
        [Description("调试")]
        public bool Debug { get; set; } = false;
        [Description("概率生成3114")]
        public bool enable3114 { get; set; } = true;
        public int chance3114 { get; set; } = 30;
        [Description("3114生成最小人数Minimum PlayerToSpwan3114")]
        public int Spwan3114MinPlayer { get; set; } = 5;
        [Description("如果人数超过这个数字，3114将会从人类中抽选(原为scp) minimun player to use Human to spwam 3114 instead of scp")]
        public int Spwan3114AtHumans { get; set; } = 10;
    }

    public class Essentails : Plugin<PluginConfig>
    {
        public static PluginConfig config;
        public override void OnEnabled()
        {
            base.OnEnabled();
            config = Config;
            EventHandler.Reg(true);
            Log.Info("\n==========白云插件启动成功=========");
            if(Config.enable3114) Log.Info($"已启用3114生成，概率{Config.chance3114},最小玩家{Config.Spwan3114MinPlayer}");
            Log.Info("==========================================\n");
        }

        public override void OnReloaded()
        {
            base.OnReloaded();
            config = Config;
            EventHandler.Reload();
            Log.Info("白云插件已重载！");
        }
        public override void OnDisabled()
        {
            base.OnDisabled();
            EventHandler.Reg(false);
            Log.Info("\n==========\n白云插件已禁用！\n==========\n");
        }
    }
    public static class EventHandler
    {
        public static void Reg(bool enable)
        {
            if (enable)
            {
                Exiled.Events.Handlers.Server.RoundStarted += Additions.Spwan3114;
            }
            else
            {
                Exiled.Events.Handlers.Server.RoundStarted -= Additions.Spwan3114;
            }
        }
        public static void Reload()
        {

        }
        
    }
    public static class Additions
    {
        public static void Spwan3114()
        {
            if (!Essentails.config.enable3114)
            {
                return;
            }
            Log.Info("准备生成3114");
            if (new Random().Next(101)<=Essentails.config.chance3114)
            {
                List<Player> players = new List<Player>();
                foreach (Player player in Player.List)
                {
                    if (Player.List.Count >= Essentails.config.Spwan3114AtHumans && player.IsHuman)
                        players.Add(player);
                    else if (Player.List.Count >= Essentails.config.Spwan3114MinPlayer && player.IsScp)
                        players.Add(player);
                }
                if (Player.List.Count< Essentails.config.Spwan3114MinPlayer)
                {
                    Log.Info("人太少了，不生成");
                    return;
                }
                Log.Info($"{players.Count}个3114备选");
                Player p = players[new Random().Next(players.Count)];
                p.RoleManager.ServerSetRole(PlayerRoles.RoleTypeId.Scp3114,PlayerRoles.RoleChangeReason.RoundStart);
                Log.Info("生成了3114");
            }
            else
            {
                Log.Info("未生成3114");
            }
            

        }
    }
}
