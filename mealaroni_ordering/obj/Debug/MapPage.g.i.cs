﻿

#pragma checksum "C:\Users\Benjamin\documents\visual studio 2013\Projects\mealaroni_ordering\mealaroni_ordering\MapPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "1261FCBCDC1EC85C026EAA22A055DAAF"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace mealaroni_ordering
{
    partial class mapPage : global::Windows.UI.Xaml.Controls.Page
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.CommandBar commandBar; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.AppBarButton dir_bnt; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.MenuFlyout myMenuFlyout; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.AppBarButton goBizBnt; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Media.Animation.Storyboard myStoryboardX; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Media.Animation.Storyboard myStoryboardY; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Media.Animation.Storyboard gridHandleStroyBoardX; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Media.Animation.Storyboard gridHandleStroyBoardY; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Media.Animation.DoubleAnimation gridHandelY; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Media.Animation.DoubleAnimation gridHandelX; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Media.Animation.DoubleAnimation wavemoveY; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Media.Animation.DoubleAnimation wavemoveX; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Grid mapgrid; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Maps.MapControl mapcontrol; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Shapes.Ellipse wavepoint; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Media.TranslateTransform sss; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.StackPanel infoPanel; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.StackPanel infoPanel_Side; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Media.TranslateTransform gridHandle; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private bool _contentLoaded;

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent()
        {
            if (_contentLoaded)
                return;

            _contentLoaded = true;
            global::Windows.UI.Xaml.Application.LoadComponent(this, new global::System.Uri("ms-appx:///MapPage.xaml"), global::Windows.UI.Xaml.Controls.Primitives.ComponentResourceLocation.Application);
 
            commandBar = (global::Windows.UI.Xaml.Controls.CommandBar)this.FindName("commandBar");
            dir_bnt = (global::Windows.UI.Xaml.Controls.AppBarButton)this.FindName("dir_bnt");
            myMenuFlyout = (global::Windows.UI.Xaml.Controls.MenuFlyout)this.FindName("myMenuFlyout");
            goBizBnt = (global::Windows.UI.Xaml.Controls.AppBarButton)this.FindName("goBizBnt");
            myStoryboardX = (global::Windows.UI.Xaml.Media.Animation.Storyboard)this.FindName("myStoryboardX");
            myStoryboardY = (global::Windows.UI.Xaml.Media.Animation.Storyboard)this.FindName("myStoryboardY");
            gridHandleStroyBoardX = (global::Windows.UI.Xaml.Media.Animation.Storyboard)this.FindName("gridHandleStroyBoardX");
            gridHandleStroyBoardY = (global::Windows.UI.Xaml.Media.Animation.Storyboard)this.FindName("gridHandleStroyBoardY");
            gridHandelY = (global::Windows.UI.Xaml.Media.Animation.DoubleAnimation)this.FindName("gridHandelY");
            gridHandelX = (global::Windows.UI.Xaml.Media.Animation.DoubleAnimation)this.FindName("gridHandelX");
            wavemoveY = (global::Windows.UI.Xaml.Media.Animation.DoubleAnimation)this.FindName("wavemoveY");
            wavemoveX = (global::Windows.UI.Xaml.Media.Animation.DoubleAnimation)this.FindName("wavemoveX");
            mapgrid = (global::Windows.UI.Xaml.Controls.Grid)this.FindName("mapgrid");
            mapcontrol = (global::Windows.UI.Xaml.Controls.Maps.MapControl)this.FindName("mapcontrol");
            wavepoint = (global::Windows.UI.Xaml.Shapes.Ellipse)this.FindName("wavepoint");
            sss = (global::Windows.UI.Xaml.Media.TranslateTransform)this.FindName("sss");
            infoPanel = (global::Windows.UI.Xaml.Controls.StackPanel)this.FindName("infoPanel");
            infoPanel_Side = (global::Windows.UI.Xaml.Controls.StackPanel)this.FindName("infoPanel_Side");
            gridHandle = (global::Windows.UI.Xaml.Media.TranslateTransform)this.FindName("gridHandle");
        }
    }
}


