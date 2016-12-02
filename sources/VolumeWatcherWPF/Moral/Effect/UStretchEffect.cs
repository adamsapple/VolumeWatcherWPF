using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;


/// <summary>
/// 
/// </summary>
namespace Moral.Effect
{
    class UStretchEffect : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty
            = ShaderEffect.RegisterPixelShaderSamplerProperty(
            "Input", typeof(UStretchEffect), 0);

        // ThresholdFactor プロパティを float 型定数レジスタ C の 0 に登録する（デフォルト値は 0.0）
        public static readonly DependencyProperty FactorProperty
            = DependencyProperty.Register(
            "Factor", typeof(double), typeof(UStretchEffect),
                new UIPropertyMetadata(0.00, PixelShaderConstantCallback(0)));

        public static readonly DependencyProperty AlphaProperty
             = DependencyProperty.Register(
             "Alpha", typeof(double), typeof(UStretchEffect),
                 new UIPropertyMetadata(1.0, PixelShaderConstantCallback(1)));

        public static readonly DependencyProperty SeedProperty
             = DependencyProperty.Register(
             "Seed", typeof(double), typeof(UStretchEffect),
                 new UIPropertyMetadata(1.0, PixelShaderConstantCallback(2)));

        public static readonly DependencyProperty WidthProperty
             = DependencyProperty.Register(
             "Width", typeof(double), typeof(UStretchEffect),
                 new UIPropertyMetadata(1.0, PixelShaderConstantCallback(3)));

        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        public double Factor
        {
            get { return (double)GetValue(FactorProperty); }
            set { SetValue(FactorProperty, value); }
        }

        public double Alpha
        {
            get { return (double)GetValue(AlphaProperty); }
            set { SetValue(AlphaProperty, value); }
        }

        public double Seed
        {
            get { return (double)GetValue(SeedProperty); }
            set { SetValue(SeedProperty, value); }
        }

        public double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        public UStretchEffect()
        {
            PixelShader pixelshader = new PixelShader();
            //var path = @"Resources/UStretchEffect.ps";
            var path = @"Resources/UStretchEffect.ps";
            var asmName = typeof(UStretchEffect).Assembly.GetName().Name;
            var uripath = $@"pack://application:,,,/{asmName};component/{path}";

            //pixelshader.UriSource = new Uri(uripath, UriKind.Relative);
            pixelshader.UriSource = new Uri(uripath, UriKind.RelativeOrAbsolute);
            pixelshader.Freeze();                   // これはなくても正常に動作するようだ

            this.PixelShader = pixelshader;
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(FactorProperty);
            UpdateShaderValue(AlphaProperty);
            UpdateShaderValue(SeedProperty);
            UpdateShaderValue(WidthProperty);
        }
    }
}
