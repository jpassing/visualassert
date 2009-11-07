﻿using System;
using System.Drawing;

namespace Cfix.Addin.Windows
{
    internal static class Chrome
    {
#if VS100
        public static readonly Color WindowBackColor = Color.FromArgb( 188, 200, 213 );
        public static readonly Bitmap CfixIcon = Icons.CfixTransparent;
#else
        public static readonly Color WindowBackColor = SystemColors.Control;
        public static readonly Bitmap CfixIcon = Icons.cfix;
#endif
    }
}