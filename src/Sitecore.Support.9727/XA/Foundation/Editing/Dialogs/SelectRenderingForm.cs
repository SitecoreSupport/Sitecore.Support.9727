namespace Sitecore.Support.XA.Foundation.Editing.Dialogs
{
    using Sitecore;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Shell.Applications.Dialogs.ItemLister;
    using Sitecore.Web.UI.HtmlControls;
    using Sitecore.Web.UI.WebControls;
    using Sitecore.XA.Foundation.SitecoreExtensions.Comparers;
    using System;
    using System.Linq;
    using System.Web.UI.WebControls;

    [UsedImplicitly]
    public class SelectRenderingForm : Sitecore.XA.Foundation.Editing.Dialogs.SelectRenderingForm
    {
        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");
            SelectRenderingOptions options = SelectItemOptions.Parse<SelectRenderingOptions>();
            base.OnLoad(e);
            this.Tabs.OnChange += new EventHandler(this.Tabs_OnChange);
            if (!Context.ClientPage.IsEvent)
            {
                base.IsOpenPropertiesChecked = Registry.GetBool("/Current_User/SelectRendering/IsOpenPropertiesChecked");
                if (options.ShowOpenProperties)
                {
                    base.OpenPropertiesBorder.Visible = true;
                    base.OpenProperties.Checked = base.IsOpenPropertiesChecked;
                }
                if (options.ShowPlaceholderName)
                {
                    base.PlaceholderNameBorder.Visible = true;
                    base.PlaceholderName.Value = options.PlaceholderName;
                }
                if (!options.ShowTree)
                {
                    base.TreeviewContainer.Class = string.Empty;
                    GridPanel parent = base.TreeviewContainer.Parent as GridPanel;
                    if (parent != null)
                    {
                        base.TreeviewContainer.Visible = false;
                        parent.SetExtensibleProperty(base.TreeviewContainer, "class", "scDisplayNone");
                        base.TreeSplitter.Visible = false;
                        parent.SetExtensibleProperty(base.TreeSplitter, "class", "scDisplayNone");
                    }
                    parent = base.Renderings.Parent as GridPanel;
                    if (parent != null)
                    {
                        base.Renderings.Visible = false;
                        parent.SetExtensibleProperty(base.Renderings, "class", "scDisplayNone");
                    }
                    TreeComparer comparer = new TreeComparer();
                    foreach (IGrouping<Item, Item> grouping in options.Items.GroupBy<Item, Item>(i => i.Parent, comparer).OrderBy<IGrouping<Item, Item>, Item>(g => g.Key, comparer).ToList<IGrouping<Item, Item>>())
                    {
                        Tab child = new Tab
                        {
                            Header = grouping.Key.DisplayName
                        };

                        Scrollbox scrollbox = new Scrollbox
                        {
                            Class = "scScrollbox scFixSize scKeepFixSize",
                            Background = "white",
                            Padding = "0px",
                            Width = new Unit(100.0, UnitType.Percentage),
                            Height = new Unit(100.0, UnitType.Percentage),
                            InnerHtml = this.RenderPreviews(grouping)
                        };
                        child.Controls.Add(scrollbox);
                        this.Tabs.Controls.Add(child);
                    }
                }
                else
                {
                    GridPanel panel2 = base.Renderings.Parent as GridPanel;
                    if (panel2 != null)
                    {
                        this.Tabs.Visible = false;
                        panel2.SetExtensibleProperty(this.Tabs, "class", "scDisplayNone");
                    }
                }
                this.SetOpenPropertiesState(options.SelectedItem);
            }
        }
    }
}
