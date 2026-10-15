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
            AssociatedObject.PreviewKeyDown += OnPreviewKeyDown;
            DataObject.AddPastingHandler(AssociatedObject, OnPasting);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewTextInput -= AssociatedObject_PreviewTextInput;
            AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;
            DataObject.RemovePastingHandler(AssociatedObject, OnPasting);
        }

        private void AssociatedObject_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            bool permiteFracionado = (AssociatedObject.Tag is bool b && b) || (AssociatedObject.Tag?.ToString() == "True");

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

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void OnPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                var textBox = sender as TextBox;
                
                if (textBox == null)
                {
                    e.CancelCommand();
                    return;
                }
                
                bool permiteFracionado = (textBox.Tag is bool b && b) || (textBox.Tag?.ToString() == "True");
                
                if (permiteFracionado)
                {
                    Regex regex = new Regex("[^0-9.]+");
                    if (regex.IsMatch(text))
                    {
                        e.CancelCommand();
                        return;
                    }
                    
                    // Verificar se já tem ponto e o texto colado também tem
                    if (textBox.Text.Contains(".") && text.Contains("."))
                    {
                        e.CancelCommand();
                    }
                }
                else
                {
                    // Modo inteiro: só números
                    Regex regex = new Regex("[^0-9]+");
                    if (regex.IsMatch(text))
                    {
                        e.CancelCommand();
                    }
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}
