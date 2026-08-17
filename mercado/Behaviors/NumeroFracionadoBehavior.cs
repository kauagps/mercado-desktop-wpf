using System;
using System.Collections.Generic;
using System.Text;

using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace mercado.Behaviors
{
    public class NumeroFracionadoBehavior : Behavior<TextBox>
    {

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewTextInput += AssociatedObject_PreviewTextInput;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewTextInput -= AssociatedObject_PreviewTextInput;
        }

        private void AssociatedObject_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            bool permiteFracionado = AssociatedObject.Tag is bool valor && valor;

            if (permiteFracionado)
            {
                Regex regex = new Regex("[^0-9.]+");
                bool temLetraOuSimbolo = regex.IsMatch(e.Text);
            
                if (e.Text == "." && AssociatedObject.Text.Contains("."))
                {
                    e.Handled = true;
                }
                else
                {
                    e.Handled = temLetraOuSimbolo;
                }
            }
            else
            {
                // MODO INTEIRO: Aceita apenas números puros
                Regex regex = new Regex("[^0-9]+");
                e.Handled = regex.IsMatch(e.Text);
            }

        }

    }
}
