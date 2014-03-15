﻿/*
 * 
 * Welcome to this tutorial.  Things you should know from a previous guide: how to work in Visual Studio or 
 * C# Express, how to code in general (general practices or naming conventions, program flow, creating 
 * classes/methods, etc.
 * 
 * Level: Beginner
 * Purpose: To get the user familiar with commands and how they are integrateable with TShock.
 * 
 * 
 */

using System;
using System.Collections.Generic;
using TShockAPI;
using System.Linq;
using System.Reflection;
using System.Text;
using Terraria;
using TerrariaApi;
using TerrariaApi.Server;

namespace TestPlugin
{
    /* This attribute is read by the server.  When it loads plugins, it only loads plugins with the same API Version.
     * When updating plugins, this needs to be changed, and often times, is the only thing that needs to change.
     * EDIT: If you're updating a plugin from pre-1,14 to 1,14 you WILL need to change more than the ApiVersion!
     */
    [ApiVersion(1, 15)]

    /*This is your main class.  This is what gets constructed immediately after being loaded by the server.
     * What happens after that is dependant on order.
     * 
     * First off, you want to make sure you "extend" the TerrariaPlugin class.  This is achieved by attaching 
     * the suffix ": TerrariaPlugin" to the end of a class.  Please read up on class hierarchy for a more detailed
     * explaination of extends.
     */
    public class TestPlugin : TerrariaPlugin
    {
        /* Override this method of the "base" class.  Overriding a method means instead of using TerrariaPlugin's
         * default method, it will use this one, which you can customize to fit your liking. 
         * This tells the server what version of the plugin is running.
         * This can be useful for figuring out why some people are experiencing issues as it tells you what version
         * they were using.
         */
        public override Version Version
        {
            /*Some people read this directly from an assemblyinfo.cs file, but this is simpler.
             */
            get { return new Version(1,0); }


            /*To read from assemblyinfo.cs: add using System.Reflection right up the top
             * Use "get { return Assembly.GetExecutingAssembly().GetName().Version; }"
             * instead of "get { return new Version("final"); }"
             */
        }

        /* Again, override this so that servers can tell what the plugin's name is. Only useful for
         * logs and displaying in the console window on startup
         */
        public override string Name
        {
            get { return "TestPlugin"; }
        }

        /* This tells the plugin who wrote the plugin.
         */
        public override string Author
        {
            get { return "Bippity"; }
        }

        /* Not sure this is even used, but its your plugin description.  This is good to do since it will
         * tell all users and people interested in your code what it does.
         */
        public override string Description
        {
            get { return "Random Commands"; }
        }

        /* The constructor for your main class.  Make sure it has an argument of Main (you can call it whatever you want)
         * And make sure you pass it to the parent using " : base()"
         */
        public TestPlugin(Main game)
            : base(game)
        {
            /*TShock by default runs at order 0.  If you wish to load before tshock (for overriding tshock handlers)
             * set Order to a positive number.  If you want to wait until tshock has finished loading, set Order < 0
             */
            Order = 1;
        }

        /*Copying format from Loganizer's "Ultrabuff" plugin
         */
        private bool[] Shine = new bool[256]; //why 256?
        private bool[] Panic = new bool[256];
        private bool[] WaterWalk = new bool[256];

        private DateTime LastCheck = DateTime.UtcNow;

        void OnUpdate(EventArgs args)
        {
            if ((DateTime.UtcNow - LastCheck).TotalSeconds > 1)
            {
                LastCheck = DateTime.UtcNow;
                for(int i = 0; i < 256; i++)
                {
                    if (Shine[i])
                        TShock.Players[i].SetBuff(11, 300, true); //60 units of 2nd param = 1 second
                    if (Panic[i])
                        TShock.Players[i].SetBuff(63, 300, true);
                    if (WaterWalk[i])
                        TShock.Players[i].SetBuff(15, 300, true);
                }
                 
            }
        }

        void OnLeave(LeaveEventArgs args)
        {
            Shine[args.Who] = false;
            Panic[args.Who] = false;
            WaterWalk[args.Who] = false;
        }

        /* This must be implemented in every plugin.  However, it is up to you whether you want to put code here.
         * This is called by the server after your plugin has been initialized (see order above).
         */
        public override void Initialize()
        {
            /* Now the actual tutorial bit.  Adding commands to TShock is extremely useful.  It allows you to hook onto
             * the very conveniant and extremely powerful command handling system.  It automatically calls your method 
             * when the command is executed and creates nice variables stored in a CommandArgs type.
             * 
             * First off, you want to call TShockAPI.Commands, then you want to get that objects member ChatCommands.
             * Then you want to add a new Command to it.
             * A command takes the form 
             *          new Command("permission", MethodToBeCalled, "commandName")
             * "permission" is the permission in the group database that corresponds to your command.
             * Leaving this blank/empty or null allows everyone to use your command.
             * You can use TShockAPI.Permissions.XXX or you can use strings. 
             * 
             * To link multiple permissions to one command use the form
             *          new Command(new List<string>() { "permission 1", "permission 2", "etc" }, MethodToBeCalled, "commandName")
             *          
             * Multiple commands can also be registered for one method and one or more permissions using the form
             *          new Command("permission", MethodToBeCalled, "commandName 1", "commandName 2", "etc")
             *          
             * You can have both multiple command names and multiple permissions by using the form
             *          new Command(new List<string>() { "permission 1", "permission 2" }, MethodToBeCalled, "commandName 1",
             *          "commandName 2")
             *          
             * MethodToBeCalled is the method you want to execute when this command is used.  It must have the argument
             * CommandArgs and no return type.
             * "commandName" is the command they would execute in game, without the "/".
             * 
             * Easy right?
             */
            Commands.ChatCommands.Add(new Command("tshock.bunny", Bunny, "bunny"));
            Commands.ChatCommands.Add(new Command("tshock.buildmode", Buildmode, "buildmode"));
            //Commands.ChatCommands.Add(new Command("tshock.clearinv", Clearinv, "clearinv")); //todo: add clearinv command

            /*
             * So this adds a new command that can be executed by typing /bunny while in game. It has no permission
             * so anyone can use it.
             * 
             * You could extend it by doing this:
             * Commands.ChatCommands.Add(new Command(new List<string>() { "bunny1", "bunny2" }, Bunny, "bunny", "rabbit"));
             */

            //hooks
            ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
            ServerApi.Hooks.ServerLeave.Register(this, OnLeave);
        }

        //Dispose Method
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.ServerLeave.Deregister(this, OnLeave);
                ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
            }
            base.Dispose(disposing);
        }

        /* This can be private, public, static, etc.
         * Notice that the method name is the same as above.
         * 
         * CommandArgs have two useful things for most operations
         * args.Player returns the player who executed the command, so you can send messages back to them,
         * kick them, ban them, do whatever you want to them.
         * args.Parameters is a list of the parameters used in game.  Things quoted in quotes are one parameter.
         * 
         * More advanced users may find having the Player object that terraria uses useful.  That is also included
         * in CommandArgs.
         */

        private void Bunny(CommandArgs args)
        {
            /* Not sure why we do this, force of habbit.
             * Check to make sure the player isn't null before we try to operate on it.
             */
            if (args.Player != null)
            {
                /* This method will set a specific buff on a player for a specified time.
                 * 40 happens to be the buff for bunnies, and the complete list can be found
                 * on the wiki.
                 * 3600 refers to 3600 seconds.  Or 6 minutes.
                 * the boolean was added recently, and I am not sure what its purpose is.
                 * Use true unless told otherwise.
                 */
                args.Player.SetBuff(40, 3600, true);

                /* Sends the defined string message to the player who executed the command
                 */
                args.Player.SendSuccessMessage("Thanks for supporting our server!");
            }
        }

        /* Will repeatedly buff player with the buffs presented below for 5 seconds.
         */
        private void Buildmode(CommandArgs args)
        {
            if (args.Player != null)
            {
                Shine[args.Player.Index] = !Shine[args.Player.Index]; //Turns method on/off
                Panic[args.Player.Index] = !Panic[args.Player.Index];
                WaterWalk[args.Player.Index] = !WaterWalk[args.Player.Index];
                
                if(Shine[args.Player.Index] && Panic[args.Player.Index])
                {
                    args.Player.SendSuccessMessage("Enabled Buildmode!");
                }
                else
                {
                    args.Player.SendSuccessMessage("Disabled Buildmode!");
                }
            }
        }
    }
}
