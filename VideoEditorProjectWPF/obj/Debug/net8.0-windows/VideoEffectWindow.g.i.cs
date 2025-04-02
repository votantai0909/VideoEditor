﻿#pragma checksum "..\..\..\VideoEffectWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "791DA54B2C7C9EDF3E0304C8D01C0339DEED0516"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using VideoEditorProjectWPF;


namespace VideoEditorProjectWPF {
    
    
    /// <summary>
    /// VideoEffectWindow
    /// </summary>
    public partial class VideoEffectWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\..\VideoEffectWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MediaElement mediaPlayer;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\..\VideoEffectWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnCut;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\..\VideoEffectWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnGray;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\..\VideoEffectWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnReverse;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\..\VideoEffectWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnSlowMotion;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\..\VideoEffectWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider VideoSlider;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "9.0.2.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/VideoEditorProjectWPF;component/videoeffectwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\VideoEffectWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "9.0.2.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.mediaPlayer = ((System.Windows.Controls.MediaElement)(target));
            return;
            case 2:
            this.btnCut = ((System.Windows.Controls.Button)(target));
            
            #line 11 "..\..\..\VideoEffectWindow.xaml"
            this.btnCut.Click += new System.Windows.RoutedEventHandler(this.BtnCut_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.btnGray = ((System.Windows.Controls.Button)(target));
            
            #line 12 "..\..\..\VideoEffectWindow.xaml"
            this.btnGray.Click += new System.Windows.RoutedEventHandler(this.BtnGray_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.btnReverse = ((System.Windows.Controls.Button)(target));
            
            #line 13 "..\..\..\VideoEffectWindow.xaml"
            this.btnReverse.Click += new System.Windows.RoutedEventHandler(this.BtnReverse_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.btnSlowMotion = ((System.Windows.Controls.Button)(target));
            
            #line 14 "..\..\..\VideoEffectWindow.xaml"
            this.btnSlowMotion.Click += new System.Windows.RoutedEventHandler(this.BtnSlowMotion_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 15 "..\..\..\VideoEffectWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.BtnReturn_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.VideoSlider = ((System.Windows.Controls.Slider)(target));
            
            #line 16 "..\..\..\VideoEffectWindow.xaml"
            this.VideoSlider.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.VideoSlider_ValueChanged);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

