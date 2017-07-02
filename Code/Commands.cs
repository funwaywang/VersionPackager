using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VPackager
{
    public static class Commands
    {
        public static readonly RoutedUICommand ImportFolder = new RoutedUICommand(Lang.GetTextWithEllipsis("Import Folder"), "ImportFolder", typeof(Commands));
        public static readonly RoutedUICommand CreateFolder = new RoutedUICommand(Lang.GetTextWithEllipsis("Create Folder"), "CreateFolder", typeof(Commands));
        public static readonly RoutedUICommand AddFiles = new RoutedUICommand(Lang.GetTextWithEllipsis("Add Files"), "AddFiles", typeof(Commands));
        public static readonly RoutedUICommand Generate = new RoutedUICommand(Lang.GetText("Generate"), "Generate", typeof(Commands));
        public static readonly RoutedUICommand Options = new RoutedUICommand(Lang.GetText("Options"), "Options", typeof(Commands));
        public static readonly RoutedUICommand ClosePopup = new RoutedUICommand(Lang.GetText("Close"), "ClosePopup", typeof(Commands));
        public static readonly RoutedUICommand TurnRelativePath = new RoutedUICommand(Lang.GetText("Turn to Relative Path"), "TurnRelativePath", typeof(Commands));
        public static readonly RoutedUICommand TurnAbsolutePath = new RoutedUICommand(Lang.GetText("Turn to Absolute Path"), "TurnAbsolutePath", typeof(Commands));
        public static readonly RoutedUICommand OpenRecent = new RoutedUICommand(Lang.GetText("Open Recent"), "OpenRecent", typeof(Commands));
        public static readonly RoutedUICommand Property = new RoutedUICommand(Lang.GetText("Property"), "Property", typeof(Commands));
        public static readonly RoutedUICommand Minimize = new RoutedUICommand(Lang.GetText("Minimize"), "Minimize", typeof(Commands));
        public static readonly RoutedUICommand Maximize = new RoutedUICommand(Lang.GetText("Maximize"), "Maximize", typeof(Commands));
        public static readonly RoutedUICommand CloseWindow = new RoutedUICommand(Lang.GetText("Close Window"), "CloseWindow", typeof(Commands));
    }
}
