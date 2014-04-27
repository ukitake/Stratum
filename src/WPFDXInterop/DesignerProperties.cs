using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics.CodeAnalysis;

namespace WPFDXInterop
{
    public static class DesignerProperties
    {
        /// <summary>
        /// Returns whether the control is in design mode (running under Blend
        /// or Visual Studio).
        /// </summary>
        /// <param name="element">The element from which the property value is
        /// read.</param>
        /// <returns>True if in design mode.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "element", Justification =
            "Matching declaration of System.ComponentModel.DesignerProperties.GetIsInDesignMode (which has a bug and is not reliable).")]
        public static bool GetIsInDesignMode(DependencyObject element)
        {
            if (!_isInDesignMode.HasValue)
            {
                _isInDesignMode =
                    (null == Application.Current) ||
                    Application.Current.GetType() == typeof(Application);
            }
            return _isInDesignMode.Value;
        }

        /// <summary>
        /// Stores the computed InDesignMode value.
        /// </summary>
        private static bool? _isInDesignMode;
    }
}
