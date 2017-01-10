using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace NDoc.Gui
{
	/// <summary>
	/// Summary description for HeaderGroupBox.
	/// </summary>
	public class HeaderGroupBox : System.Windows.Forms.GroupBox
	{
		private int padding = 0;

		/// <summary>
		/// 
		/// </summary>
		public HeaderGroupBox()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		[Category("Appearance")]
		[Description("Adds some extra spacing around the header")]
		public int Padding
		{
			get { return padding; }
			set
			{
				if (value != padding)
				{
					padding = value;
					Invalidate();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			using ( StringFormat format = new StringFormat() )
			{			
				format.Trimming = StringTrimming.Character;
				format.Alignment = StringAlignment.Near;

				if (this.RightToLeft == RightToLeft.Yes)
				{
					format.FormatFlags = format.FormatFlags | StringFormatFlags.DirectionRightToLeft;
				}

				Rectangle textRectangle = Rectangle.Inflate( ClientRectangle, -padding, 0 );

				SizeF stringSize = e.Graphics.MeasureString(Text, Font, textRectangle.Size, format);

				if (Enabled)
				{
					using( Brush br = new SolidBrush(ForeColor) )
						e.Graphics.DrawString(Text, Font, br, textRectangle, format);
				}
				else
				{
					ControlPaint.DrawStringDisabled(e.Graphics, Text, Font, BackColor, textRectangle, format);
				}


				Point lineLeft = new Point(textRectangle.Left, textRectangle.Top + (int)(Font.Height / 2f));
				Point lineRight = new Point(textRectangle.Right, textRectangle.Top + (int)(Font.Height / 2f));

				if (this.RightToLeft != RightToLeft.Yes)
				{
					lineLeft.X += (int)stringSize.Width;
				}
				else
				{
					lineRight.X -= (int)stringSize.Width;
				}

				using ( Pen forePenDark = new Pen(ControlPaint.Dark( BackColor ), SystemInformation.BorderSize.Height) )
				{
					if (FlatStyle == FlatStyle.Flat)
					{
						e.Graphics.DrawLine(forePenDark, lineLeft, lineRight);                        	
					}
					else
					{
						using ( Pen forePen = new Pen(ControlPaint.LightLight( BackColor ), SystemInformation.BorderSize.Height) )
						{
							e.Graphics.DrawLine(forePenDark, lineLeft, lineRight);                        	
							lineLeft.Offset(0, (int)Math.Ceiling((float)SystemInformation.BorderSize.Height / 2f));
							lineRight.Offset(0, (int)Math.Ceiling((float)SystemInformation.BorderSize.Height / 2f));
							e.Graphics.DrawLine(forePen, lineLeft, lineRight);
						}
					}
				}
			}
		}
	}
}
