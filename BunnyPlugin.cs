/*
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
using System.ComponentModel;
using TShockAPI;
using TShockAPI.Hooks;
using System.Linq;
using System.Reflection;
using System.Text;
using Terraria;
using TerrariaApi;
using TerrariaApi.Server;

namespace BunnyPlugin
{
    /* This attribute is read by the server.  When it loads plugins, it only loads plugins with the same API Version.
     * When updating plugins, this needs to be changed, and often times, is the only thing that needs to change.
     * EDIT: If you're updating a plugin from pre-1,14 to 1,14 you WILL need to change more than the ApiVersion!
     */
    [ApiVersion(1, 22)]

    /*This is your main class.  This is what gets constructed immediately after being loaded by the server.
     * What happens after that is dependant on order.
     * 
     * First off, you want to make sure you "extend" the TerrariaPlugin class.  This is achieved by attaching 
     * the suffix ": TerrariaPlugin" to the end of a class.  Please read up on class hierarchy for a more detailed
     * explaination of extends.
     */
    public class BunnyPlugin : TerrariaPlugin
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
            get { return "BunnyPlugin"; }
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
            get { return "Bunny."; }
        }

        /* The constructor for your main class.  Make sure it has an argument of Main (you can call it whatever you want)
         * And make sure you pass it to the parent using " : base()"
         */
        public BunnyPlugin(Main game)
            : base(game)
        {
            /*TShock by default runs at order 0.  If you wish to load before tshock (for overriding tshock handlers)
             * set Order to a positive number.  If you want to wait until tshock has finished loading, set Order < 0
             */
            Order = 1;
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
           
            /*
             * So this adds a new command that can be executed by typing /bunny while in game. It has no permission
             * so anyone can use it.
             * 
             * You could extend it by doing this:
             * Commands.ChatCommands.Add(new Command(new List<string>() { "bunny1", "bunny2" }, Bunny, "bunny", "rabbit"));
             */

            PlayerHooks.PlayerPostLogin += PostLogin;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                PlayerHooks.PlayerPostLogin -= PostLogin;
            }
            base.Dispose(disposing);
        }

        private void PostLogin(PlayerPostLoginEventArgs args)
        {
            if (args.Player != null)
            {
                TSPlayer player = args.Player;
                if (player.Group.HasPermission("tshock.bunny"))
                buff(args.Player);
            }
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
                buff(args.Player);
            }
        }

        private void buff(TSPlayer player)
        {
            player.SetBuff(40, 3600, true);
            player.SendSuccessMessage("Thanks for supporting our server!");
        }
    }
}
