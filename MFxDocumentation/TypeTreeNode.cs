// -------------------------------------------------------------------------
//  <copyright file="TypeTreeNode.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------- 
namespace MFxDocumentation
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// This class represents everything we care about the Type object.
    /// This is node in the tree.
    /// </summary>
    internal class TypeTreeNode
    {
        /// <summary>
        /// Initializes a new instance of the TypeTreeNode class.
        /// </summary>
        public TypeTreeNode()
        {
            this.Children = new Dictionary<string, TypeTreeNode>();
        }

        /// <summary>Gets or sets base system type represented by this TypeTreeNode.</summary>
        public Type RawType { get; set; }

        /// <summary>Gets or sets the name of the type object.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the depth of the node in the tree.</summary>
        public int Depth { get; set; }

        /// <summary>Gets or sets the number of children directly attached to this tree node.</summary>
        public int DirectChildren { get; set; }

        /// <summary>Gets or sets the number of children directly and indirectly attached to this tree node.</summary>
        public int TotalChildren { get; set; }

        /// <summary>Gets or sets the Dictionary of attached children.</summary>
        public Dictionary<string, TypeTreeNode> Children { get; set; }

        /// <summary>
        /// Override the ToString() to get useful display.
        /// </summary>
        /// <returns>String for rendering.</returns>
        public override string ToString()
        {
            string returnString;
            if (this.RawType != null)
            {
                returnString = string.Format(
                    "{0}\t{1}\t{2}\t{3}\t({4}) ", 
                    this.Name, 
                    this.Depth, 
                    this.DirectChildren, 
                    this.TotalChildren, 
                    this.RawType.FullName);
                StringBuilder sb = new StringBuilder(returnString);
                Type t = this.RawType;
                while (t != null)
                {
                    sb.Append("::" + t.Name);
                    t = t.BaseType;
                }

                return sb.ToString();
            }
            else 
            {
                returnString = string.Format(
                    "{0}\t{1}\t{2}\t{3}", 
                    this.Name, 
                    this.Depth, 
                    this.DirectChildren, 
                    this.TotalChildren);
                return returnString;
            }
        }
    }
}
