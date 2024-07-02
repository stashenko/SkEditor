using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using SkEditor.API;
using SkEditor.Controls.Docs;
using SkEditor.Utilities;
using SkEditor.Utilities.Editor;
using SkEditor.Utilities.Files;
using SkEditor.Utilities.Projects;
using SkEditor.Utilities.Syntax;
using SkEditor.Views;
using SkEditor.Views.Generators;
using SkEditor.Views.Generators.Gui;
using SkEditor.Views.Settings;

namespace SkEditor.Controls;
public partial class MainMenuControl : UserControl
{

    public MainMenuControl()
    {
        InitializeComponent();

        AssignCommands();
    }

    private void AssignCommands()
    {
        MenuItemNew.Command = new RelayCommand(FileHandler.NewFile);
        MenuItemOpen.Command = new RelayCommand(FileHandler.OpenFile);
        MenuItemOpenFolder.Command = new RelayCommand(() => ProjectOpener.OpenProject());
        MenuItemSave.Command = new RelayCommand(FileHandler.SaveFile);
        MenuItemSaveAs.Command = new RelayCommand(FileHandler.SaveAsFile);
        MenuItemPublish.Command = new RelayCommand(() => new PublishWindow().ShowDialog(SkEditorAPI.Windows.GetMainWindow()));

        MenuItemClose.Command = new RelayCommand(FileCloser.CloseCurrentFile);
        MenuItemCloseAll.Command = new RelayCommand(FileCloser.CloseAllFiles);
        MenuItemCloseAllExceptCurrent.Command = new RelayCommand(FileCloser.CloseAllExceptCurrent);
        MenuItemCloseAllUnsaved.Command = new RelayCommand(FileCloser.CloseUnsaved);
        MenuItemCloseAllLeft.Command = new RelayCommand(FileCloser.CloseAllToTheLeft);
        MenuItemCloseAllRight.Command = new RelayCommand(FileCloser.CloseAllToTheRight);

        MenuItemCopy.Command = new RelayCommand(() => SkEditorAPI.Files.GetCurrentOpenedFile().Editor.Copy());
        MenuItemPaste.Command = new RelayCommand(() => SkEditorAPI.Files.GetCurrentOpenedFile().Editor.Paste());
        MenuItemCut.Command = new RelayCommand(() => SkEditorAPI.Files.GetCurrentOpenedFile().Editor.Cut());
        MenuItemUndo.Command = new RelayCommand(() => SkEditorAPI.Files.GetCurrentOpenedFile().Editor.Undo());
        MenuItemRedo.Command = new RelayCommand(() => SkEditorAPI.Files.GetCurrentOpenedFile().Editor.Redo());
        MenuItemDelete.Command = new RelayCommand(() => SkEditorAPI.Files.GetCurrentOpenedFile().Editor.Delete());
        MenuItemGoToLine.Command = new RelayCommand(() => SkEditorAPI.Windows.ShowWindow(new GoToLine()));
        MenuItemTrimWhitespaces.Command = new RelayCommand(() => CustomCommandsHandler.OnTrimWhitespacesCommandExecuted(SkEditorAPI.Files.GetCurrentOpenedFile().Editor?.TextArea));

        MenuItemDuplicate.Command = new RelayCommand(() => CustomCommandsHandler.OnDuplicateCommandExecuted(SkEditorAPI.Files.GetCurrentOpenedFile().Editor.TextArea));
        MenuItemComment.Command = new RelayCommand(() => CustomCommandsHandler.OnCommentCommandExecuted(SkEditorAPI.Files.GetCurrentOpenedFile().Editor.TextArea));

        MenuItemRefreshSyntax.Command = new RelayCommand(async () => await SyntaxLoader.RefreshSyntaxAsync());

        MenuItemSettings.Command = new RelayCommand(() => new SettingsWindow().ShowDialog(SkEditorAPI.Windows.GetMainWindow()));
        MenuItemGenerateGui.Command = new RelayCommand(() => new GuiGenerator().ShowDialog(SkEditorAPI.Windows.GetMainWindow()));
        MenuItemGenerateCommand.Command = new RelayCommand(() => new CommandGenerator().ShowDialog(SkEditorAPI.Windows.GetMainWindow()));
        MenuItemRefactor.Command = new RelayCommand(() => new RefactorWindow().ShowDialog(SkEditorAPI.Windows.GetMainWindow()));
        MenuItemMarketplace.Command = new RelayCommand(() => new MarketplaceWindow().ShowDialog(SkEditorAPI.Windows.GetMainWindow()));

        MenuItemDocs.Command = new RelayCommand(AddDocsTab);
    }

    public void AddDocsTab()
    {
        SkEditorAPI.Files.AddCustomTab("Documentation", new DocumentationControl());
    }

    public void ReloadAddonsMenus()
    {
        bool hasAnyMenu = false;
        AddonsMenuItem.Items.Clear();
        foreach (IAddon addon in SkEditorAPI.Addons.GetAddons(IAddons.AddonState.Enabled))
        {
            var items = addon.GetMenuItems();
            if (items.Count <= 0)
                continue;

            hasAnyMenu = true;
            var menuItem = new MenuItem()
            {
                Header = addon.Name,
                Icon = new IconSourceElement()
                {
                    IconSource = addon.GetAddonIcon(),
                    Width = 20,
                    Height = 20
                }
            };

            if (addon.GetSettings().Count > 0)
            {
                menuItem.Items.Add(new MenuItem()
                {
                    Header = Translation.Get("WindowTitleSettings"),
                    Command = new RelayCommand(() =>
                    {
                        new SettingsWindow().ShowDialog(MainWindow.Instance);
                        SettingsWindow.NavigateToPage(typeof(CustomAddonSettingsPage));
                        CustomAddonSettingsPage.Load(addon);
                    }),
                    Icon = new IconSourceElement()
                    {
                        IconSource = new SymbolIconSource() { Symbol = Symbol.Setting, FontSize = 20 },
                        Width = 20,
                        Height = 20
                    }
                });
                menuItem.Items.Add(new Separator());
            }

            foreach (MenuItem sub in items)
                menuItem.Items.Add(sub);

            AddonsMenuItem.Items.Add(menuItem);
        }

        AddonsMenuItem.Items.Add(new Separator());
        AddonsMenuItem.Items.Add(new MenuItem()
        {
            Header = Translation.Get("MenuHeaderManageAddons"),
            Command = new RelayCommand(() =>
            {
                new SettingsWindow().ShowDialog(MainWindow.Instance);
                SettingsWindow.NavigateToPage(typeof(AddonsPage));
            }),
            Icon = new IconSourceElement()
            {
                IconSource = new SymbolIconSource()
                {
                    Symbol = Symbol.Manage,
                    FontSize = 20
                },
                Width = 20,
                Height = 20
            }
        });

        AddonsMenuItem.IsVisible = hasAnyMenu;
    }
}
