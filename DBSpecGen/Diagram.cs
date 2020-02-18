using System;
using System.Xml;
using System.Collections;
using System.Globalization;


namespace MinimalCrossing
{
	/// <summary>
	/// Summary description for Diagram.
	/// </summary>
	public class ConnectedGraph
	{
		private bool _initialized;
		private SortedList _icons;
		private int _height;
		private int _width;
		private int _coordinateSize;
		private int _top;
		private int _left;
		private int _horizontalSpace;
		private int _verticalSpace;
		private int _iconsPerRow;
		private int _numLinkedIcons;
		private int _numSiteRows;
		private int _numNoLinkIcons;
		private double _siteFactor;
		private System.Random _random;
		private Array _siteMatrix;
		private int _maxY;
		private int _minY;
		private int _maxX;
		private int _minX;
		private int _costFactor;
		private bool _bVerbose;
		private int _explored;
		private bool _bAllowOverlap;
		private int _numLinks;

		public ConnectedGraph(
			int top, 
			int left, 
			int horizontalSpace, 
			int verticalSpace, 
			int iconsPerRow,
			bool verbose,
			bool allowOverlap,
			int seed)
		{
			_initialized = false;
			_icons = new SortedList();
			_height = 0;
			_width = 0;
			_coordinateSize = 0;
			_top = top;
			_left = left;
			_horizontalSpace = horizontalSpace + horizontalSpace % 2;
			_verticalSpace = verticalSpace + verticalSpace % 2;
			_iconsPerRow = iconsPerRow;
			_numLinkedIcons = 0;
			_numSiteRows = 0;
			_numNoLinkIcons = 0;
			_siteFactor = 2;
			_random = new System.Random(seed);
			_siteMatrix = null;
			_bVerbose = verbose;
			_bAllowOverlap = allowOverlap;
			_numLinks = 0;
		}

		public bool PlaceIcons(DBSpecGen.DBSpecGen ui)
		{
			_explored = 0;
			DateTime start = DateTime.Now;

			InitializeIcons();
			InitializePossibleLocations();

            bool cancel = false;
            if (_bVerbose)
            {
                ulong configs = configurations(_iconsPerRow * _numSiteRows, _icons.Count);
                DBSpecGen.DBSpecGen.ShowProgress("Generating datamodel diagram: " +_icons.Count + " icons, " + (_iconsPerRow * _numSiteRows) + " locations.", -1, false, ui, out cancel);
                DBSpecGen.DBSpecGen.ShowProgress("  possible configurations: " + ((configs == 0) ? "more than 18446744073709551615 (ulong overflow)" : configs.ToString(CultureInfo.CurrentCulture)), -1, false, ui, out cancel);
                if (cancel) 
                {
                    DBSpecGen.DBSpecGen.ShowProgress("Operation canceled", 0, true, ui, out cancel);
                    return false;
                }
            }
            
			InitializeLinkedIcons(_icons);
			_initialized = true;

			Randomize(_icons);

			// use simulated annealing here to try and minimize both the 
			// length of the lines connecting the icons, and the number of
			// crossings of the line segments...		
			if (this._numLinkedIcons > 0)
			{
                if (anneal(_icons, ui) == -1)
                {
                    return false;
                }
			}

			// move the diagram up as far as we can into the upper left corner.
			ShiftDiagram();

			// put the icons with no links along the bottom.
			PlaceUnlinkedIcons();

			// set the size of out canvas.
			SetCanvasBoundaries();

            if (_bVerbose)
            {
                DBSpecGen.DBSpecGen.ShowProgress("  finished, time: " + (DateTime.Now - start), -1, false, ui, out cancel);
                DBSpecGen.DBSpecGen.ShowProgress("  explored configurations: " + _explored, -1, false, ui, out cancel);
                if (cancel) 
                {
                    DBSpecGen.DBSpecGen.ShowProgress("Operation canceled", 0, true, ui, out cancel);
                    return false;
                }
            }
            return true;
		}

		

		private double anneal(SortedList icons, DBSpecGen.DBSpecGen ui)
		{
			double lowestEnergy = energy(icons); 
			double currentEnergy = lowestEnergy;
			double delta = 0;
			int lastCost = Cost(icons);
			int variationLimit =  30 * _numLinkedIcons; // maximum number of variations at a fixed temperature
			int successLimit   =  10 * _numLinkedIcons; // maximum number of successful variations at a fixed temp
			
			// first we need to figure out what the starting temperature should be.
			// this is the average difference in length for a few different configurations.
			// let's take 10 configurations, compute the delta from one to the next, and
			// take that average.
			double avg = 0;
			int numConfigs = 10;
			Point dummy = null;
			double energy1 = 0;
			SortedList iconsCopy = icons;
			double energy2 = energy(iconsCopy);
			for(int i=0; i<numConfigs; ++i)
			{
				energy1 = energy2;
				wiggle(iconsCopy, ref dummy);
				energy2 = energy(iconsCopy);
				avg += Math.Abs(energy1 - energy2);
			}

            bool cancel = false;
            if (_bVerbose)
            {
                DBSpecGen.DBSpecGen.ShowProgress("  successLimit, variationLimit: " + successLimit + "," + variationLimit, -1, false, ui, out cancel);
                if (cancel) 
                {
                    DBSpecGen.DBSpecGen.ShowProgress("Operation canceled", 0, true, ui, out cancel);
                    return -1;
                }
            }
			
			// ok, let's set the temperature at the 
			// average difference of these configurations 
			double temperature = avg / numConfigs;

			// keep track of number of zero, positive, 
			// and negative energy steps
			int numDeltaZero = 0;
			int numDeltaPlus = 0;
			int numDeltaMinus = 0;
			double avgEnergyUp = 0;
			double avgEnergyDown = 0;
			
			// take no more than 100 temperature steps...
			for(int tempStep = 0; tempStep < 100; ++tempStep) 
			{
				// to print out the diagram as it cools so we can look at intermediate states.
				// need to set a breakpoint here if you want to do this.  should comment out for release.
				//string s = "<html xmlns:v='urn:schemas-microsoft-com:vml'><body>" + GenerateHtmlString() + "</body></html>";
				//System.IO.StreamWriter sw = System.IO.File.CreateText("dork.htm");
				//sw.Write(s);
				//sw.Close();

				// decrease the temperature
				temperature *= .90;

				// how many successful moves we have made at this temperature
				int successfulMoves = 0;
				int failedMoves = 0;
				numDeltaZero = 0;
				numDeltaPlus = 0;
				numDeltaMinus = 0;
				avgEnergyUp = 0;
				avgEnergyDown = 0;

				// for each temperature, try variationLimit different moves
				for(int i=0; i < variationLimit; ++i)
				{
					// wiggle the current configuration...
					Point movedFrom = null;
					Icon[] iconsMoved = wiggle(icons, ref movedFrom);
					_explored++;

					// ok, we moved an icon.  should we keep it?
					currentEnergy = energy(icons);
					delta = currentEnergy - lowestEnergy;
					double probability = _random.NextDouble();
					double boltzmann = Math.Exp(-delta/temperature);

					// here's the key.  we never move back if delta is negative.
					// but sometimes, we even keep it if delta is positive.  
					// the probability that we keep an uphill evolution
					// depends exponentially on how big delta is compared 
					// with the temperature
					bool bMoveBack = (probability < boltzmann) ? false : true;
					
					if (bMoveBack)
					{
						// ah well.  didn't work this time.
						++failedMoves;

						// move the icon back where it was.
						if (iconsMoved[1] != null)
						{
							SwapIcons(iconsMoved[0], iconsMoved[1], icons);
						}
						else
						{
							MoveIcon(iconsMoved[0], movedFrom, icons);
						}
						continue;
					}
					else
					{
						if (delta == 0) 
						{
							++numDeltaZero;
						}
						else if (delta < 0) 
						{
							++numDeltaMinus;
							avgEnergyDown += delta;
						}
						else if (delta > 0) 
						{
							++numDeltaPlus;
							avgEnergyUp += delta;
						}
						lowestEnergy = currentEnergy;
						++successfulMoves;
						if(successfulMoves > successLimit)
						{
							break;
						}
					}
				}

                if (_bVerbose)
                {
                    DBSpecGen.DBSpecGen.ShowProgress("  (+[avg],-[avg],0,T,E) = (" + numDeltaPlus + "[" + (int)(avgEnergyUp/numDeltaPlus) + "], " + numDeltaMinus + "[" + (int)(avgEnergyDown/numDeltaMinus) + "], " + numDeltaZero + ", " + (int)temperature + ", " + (int)lowestEnergy + ")", -1, false, ui, out cancel);
                    if (cancel) 
                    {
                        DBSpecGen.DBSpecGen.ShowProgress("Operation canceled", 0, true, ui, out cancel);
                        return -1;
                    }
                }

				// if we didn't get very many successful moves 
				// at this temperature, let's give up.  we are cold enough.
				// that is, when the temperature is very low, we almost never
				// take an uphill step, so if we are near a minimum, 
				// we will be trapped in it if the tempertature is low.
				//if ((double)successfulMoves / (double)successLimit < .1)
				if ((double)numDeltaMinus / (double)successLimit < .1)
				{
					break;
				}

				/*
				// see if most of the moves were zero delta. zero delta moves 
				// happen a lot once the configuration is pretty much frozen,
				// where you might have one icon flipping back and forth
				// between equivalent energy states.  basically we are in
				// a metastable state, where there is no cost for an icon
				// hopping back and forth. 
				if (numDeltaZero > numDeltaMinus)
				{
					break;
				}
				*/
			}
			return temperature;
		}

		private double energy(SortedList icons)
		{
			return Length(icons) + Cost(icons);
		}

		private Icon[] wiggle(SortedList icons, ref Point movedFrom)
		{
			if (this._numLinkedIcons == 0)
			{
				return null;
			}

			// pick a random icon
			int j = this._random.Next(icons.Count);
			Icon iconToMove = (Icon)icons.GetByIndex(j);
			if(!IconHasLinks(iconToMove))
			{
				// try again...
				return wiggle(icons, ref movedFrom);
			}

			// remember where we moved from...
			movedFrom = iconToMove._loc;
			Point moveTo = null;
	
			//if (_random.Next(2) == 1)
			if (true)
			{
				// randomly move icon up, down, left, or right
				double x = iconToMove._loc.X;
				double y = iconToMove._loc.Y;
				switch(_random.Next(4))
				{
					case 0:
						x += _horizontalSpace; // right
						y -= _verticalSpace / 2;
						//y += _random.Next(2) == 1 ? _verticalSpace / 2 : - _verticalSpace / 2;
						break;
					case 1:
						x -= _horizontalSpace; // left
						y -= _verticalSpace / 2;
						//y += _random.Next(2) == 1 ? _verticalSpace / 2 : - _verticalSpace / 2;
						break;
					case 2:
						y += _verticalSpace;   // down
						break;
					case 3:
						y -= _verticalSpace;   // up
						break;
					default:
						return null;
				}
				if(x > _maxX || x < _minX || y > _maxY || y < _minY)
				{
					// we picked a point outside of our box.  try again.
					return wiggle(icons, ref movedFrom);
				}
				moveTo = new Point(x,y);
			}
			/*
			else
			{
				// pick a random location to move our icon...
				int roman    = _random.Next(_numSiteRows);
				int catholic = _random.Next(_iconsPerRow);
				moveTo = (Point)_siteMatrix.GetValue(roman, catholic);
			}
			*/
		
			// find out if anyone is located at moveTo...
			Icon iconAtMoveTo = null;
			foreach (DictionaryEntry d in icons)
			{
				Icon i = (Icon)d.Value;
				if(i._loc.X == moveTo.X && i._loc.Y == moveTo.Y)
				{
					iconAtMoveTo = i;
					break;
				}
			}

			// move or swap?
			if (iconAtMoveTo != null)
			{
				SwapIcons(iconToMove, iconAtMoveTo, icons);
			}
			else
			{
				MoveIcon(iconToMove, moveTo, icons);
			}
			Icon[] iconsMoved = {iconToMove, iconAtMoveTo};
			return iconsMoved;
		}

		private void SwapIcons(Icon i, Icon j, SortedList icons)
		{	
			Point p = i._loc;
			MoveIcon(i, j._loc, icons);
			MoveIcon(j, p, icons);
		}

		private void InitializeLinkedIcons(SortedList icons)
		{
			// first put all icons at the origin to unoccupy our possible sites.
			int i=0, j=0;
			foreach (DictionaryEntry d in icons)
			{
				Icon icon = (Icon)d.Value;
				if(IconHasLinks(icon))
				{
					MoveIcon(icon, (Point)_siteMatrix.GetValue(i,j), icons);
					++j;
					if(j >= this._iconsPerRow)
					{
						j = 0;
						i++;
					}
				}
			}

			// now place the links
			foreach (DictionaryEntry  d in icons)
			{
				Icon from = (Icon)d.Value;
				if(from._pointsTo != null)
				{
					from.ClearLineSegments();
					for(int k=0; k<from._pointsTo.Length; ++k)
					{
						if(_icons.Contains(from._pointsTo[k]))
						{
							Icon to = (Icon)_icons[from._pointsTo[k]];
							LineSegment ls = new LineSegment(from._loc, to._loc, from.Name + " --> " + to.Name, from.Name, to.Name);
							from.AddLineSegment(ls);
						}
					}
				}
				_numLinks += from._lineSegments.Count;
			}
		}

		private bool Randomize(SortedList icons)
		{
			// first put all icons at the origin to unoccupy our possible sites.
			foreach (DictionaryEntry d in icons)
			{
				Icon icon = (Icon)d.Value;
				if(IconHasLinks(icon))
				{
					MoveIcon(icon,new Point(0,0),icons);
				}
			}

			// place the linked icons 
			// randomly on our lattice.
			foreach (DictionaryEntry d in icons)
			{
				Icon icon = (Icon)d.Value;
				if(IconHasLinks(icon))
				{
					// pick a random spot 
					bool placed = false;
					while(!placed)
					{
						int i = _random.Next(_numSiteRows);
						int j = _random.Next(_iconsPerRow);
						Point p = (Point)_siteMatrix.GetValue(i,j);
						if(!IsOccupied(p, icons))
						{
							MoveIcon(icon,p,icons);
							placed = true;
						}
					}
				}
			}
			return true;
		}

		public bool AddIcon(Icon i)
		{
			if(i.Name.Length == 0 || _icons.Contains(i.Name))
			{
				return false;
			}
			_icons.Add(i.Name,i);
			return true;
		}

		public string GenerateHtmlString()
		{
			if(!_initialized) 
			{
				return "<!-- must call Initialize() first -->";
			}
			string str = "<v:group  xmlns:v='urn:schemas-microsoft-com:vml' coordsize='" + _coordinateSize + "px,";
			str += _coordinateSize + "px' coordorig='0px,0px' style='position:relative;top:";
			str += _top + "px;left:" + _left + "px;width:" + _width + "px;height:" + _height +"'>";
			foreach (DictionaryEntry  d in _icons)
			{
				Icon icon = (Icon)d.Value;  
				str += icon.GenerateHtmlString();
			}
			str += "</v:group>";
			return str;
		}
		
		private bool IsOccupied(Point p, SortedList icons)
		{
			foreach (DictionaryEntry d in icons)
			{
				Icon i = (Icon)d.Value;
				if(i._loc.X == p.X && i._loc.Y == p.Y)
				{
					return true;
				}
			}
			return false;
		}

		private void InitializePossibleLocations()
		{
			// how many icons with links are there?
			foreach (DictionaryEntry  d in _icons)
			{
				Icon icon = (Icon)d.Value;
				if(IconHasLinks(icon))
				{
					_numLinkedIcons++;
				}
				else
				{	
					_numNoLinkIcons++;
				}
			}

			// how many rows of possible sites will we have?
			_numSiteRows = (int)(Math.Ceiling((double)_numLinkedIcons/_iconsPerRow) * _siteFactor);
			
			// set the minimum number of rows 
			if (_numSiteRows < 3)
			{
				_numSiteRows = 3;
			}

			// fill in the array of sites possible to use for annealing...
			_siteMatrix = Array.CreateInstance( typeof(Point), _numSiteRows, _iconsPerRow);
			int x = _horizontalSpace;
			int y = _verticalSpace;
			for(int i=0; i<_numSiteRows; ++i)
			{
				x = _horizontalSpace;	
				for(int j=0; j<_iconsPerRow; ++j)
				{
					if (j % 2 == 0) 
					{
						y += _verticalSpace / 2;
					}
					else
					{
						y -= _verticalSpace / 2;
					}
					_siteMatrix.SetValue(new Point(x,y), i, j);
					x += _horizontalSpace;
				}
				y += _verticalSpace;
			} 

			// get the boundaries of our set of points...
			_minX = x;
			_minY = y;
			_maxX = 0;
			_maxY = 0;
			for(int i=0; i<_numSiteRows; ++i)
			{	
				for(int j=0; j<_iconsPerRow; ++j)
				{
					Point p = (Point)_siteMatrix.GetValue(i, j);
					_maxX = (p.X > _maxX) ? (int)p.X : _maxX;
					_maxY = (p.Y > _maxY) ? (int)p.Y : _maxY;
					_minX = (p.X < _minX) ? (int)p.X : _minX;
					_minY = (p.Y < _minY) ? (int)p.Y : _minY;
				}
			}


			// we multiply the cost per crossing, overlapping lines, etc, by
			// this number to try and normalize crossings with the cost
			// of line lengths, since we are trying to minimize both crossings
			// and the lengths of all the lines.
			_costFactor = ((_maxX - _minX) + (_maxY - _minY)) / 2;
		}

		private void ShiftDiagram()
		{
			// first get the minimum x,y positions
			// for the linked icons placed so far, 
			// see if we can move it up and over into
			// the upper left hand corner of the page further.
			double minX = this._maxX;
			double minY = this._maxY;
			foreach (DictionaryEntry  d in _icons)
			{
				Icon icon = (Icon)d.Value;
				if (IconHasLinks(icon))
				{
					minX = (icon._loc.X < minX) ? icon._loc.X : minX;
					minY = (icon._loc.Y < minY) ? icon._loc.Y : minY;
				}
			}

			double deltaX = minX - _horizontalSpace;
			double deltaY = minY - _verticalSpace;

			// now move each icon
			if (deltaX != 0 || deltaY != 0)
			{
				foreach (DictionaryEntry  d in _icons)
				{
					Icon icon = (Icon)d.Value;
					if (IconHasLinks(icon))
					{
						MoveIcon(icon, new Point(icon._loc.X - deltaX, icon._loc.Y - deltaY), _icons);
					}
				}
			}
		}

		private void SetCanvasBoundaries()
		{
			double maxY = 0;
			double maxX = 0;
			foreach (DictionaryEntry  d in _icons)
			{
				Icon icon = (Icon)d.Value;
				maxX = (icon._loc.X > maxX) ? icon._loc.X : maxX;
				maxY = (icon._loc.Y > maxY) ? icon._loc.Y : maxY;
			}

			_width = (int)maxX + _horizontalSpace;
			_height = (int)maxY + _verticalSpace;
			_coordinateSize = 1000;
		}

		private void PlaceUnlinkedIcons()
		{
			// first get the maximum y position 
			// for the linked icons placed so far...
			double maxY = 0;
			foreach (DictionaryEntry  d in _icons)
			{
				Icon icon = (Icon)d.Value;
				maxY = (icon._loc.Y > maxY) ? icon._loc.Y : maxY;
			}
			
			double x = _horizontalSpace;
			double y = _verticalSpace + maxY;
			int iconsInRow = 0;
            bool shiftDown = true;

			// place icons that have no links to or from.
			foreach (DictionaryEntry  d in _icons)
			{
				if(iconsInRow >= _iconsPerRow)
				{
					x = _horizontalSpace;
					y += _verticalSpace;
					iconsInRow = 0;
                    shiftDown = true;
				}
				Icon i = (Icon)d.Value;
				if(!IconHasLinks(i))
				{
                    if (shiftDown)
                    {
                        y += _verticalSpace / 2;
                    }
                    else
                    {
                        y -= _verticalSpace / 2;
                    }

					MoveIcon(i, new Point(x,y),_icons);
					x += _horizontalSpace;
                    shiftDown = !shiftDown;
					++iconsInRow;
				}
			}
		}
		
		private int Cost(SortedList iconList)
		{
			int cost = 0;

			// put all icons with links in an array
			int i = 0, j = 0;
			Icon[] icons = new Icon[_numLinkedIcons];
			foreach (DictionaryEntry  d in iconList)
			{
				Icon icon = (Icon)d.Value;
				if (icon._hasLinks)
				{
					icons[i] = icon;
					++i;
				}
			}

			for(i=0; i<icons.Length; ++i)
			{
				foreach (LineSegment ls1 in icons[i]._lineSegments)
				{
					for(j=0; j<icons.Length; ++j)
					{
						if(i == j) continue;
						
						// if j < i, then icons[j] is an icon that 
						// we have already looked at for detecting 
						// crossings and overlappings.
						// be smart and don't check twice.
						if(j > i)
						{
							foreach (LineSegment ls2 in icons[j]._lineSegments)
							{
								if(ls1.To == ls2.To && ls1.From == ls2.From) continue;
								if(ls1.To == ls2.From && ls1.From == ls2.To) continue;
							
								// do these line segments cross?
								if(ls1.Crosses(ls2)) 
								{
									cost += 1;
								}

								// do these line segments overlap?
								if(!_bAllowOverlap && ls1.Overlaps(ls2)) 
								{
									cost += 1;
								}

							}
						}

						// is icon[j] contained in a line segment of icon[i]?
						if(!_bAllowOverlap && ls1.Contains(icons[j]._loc)) 
						{
							cost += 1;
						}
					}
				}
			}

			cost *= _costFactor;
			return cost;
		}

		private double Length(SortedList icons)
		{
			double length = 0;
			foreach (DictionaryEntry d in icons)
			{
				Icon icon = (Icon)d.Value;
				foreach (LineSegment ls in icon._lineSegments)
				{
					length += ls.Length;
				}
			}
			return length;
		}

		private bool MoveIcon(Icon iconToMove, Point moveToPoint, SortedList icons)
		{
			// move the icon
			iconToMove.MoveTo(moveToPoint);

			// move endpoints of links pointing to this icon
			foreach (DictionaryEntry  d in icons)
			{
				Icon from = (Icon)d.Value;
				foreach (LineSegment ls in from._lineSegments)
				{
					if(ls.To == iconToMove.Name)
					{
						ls.End = moveToPoint;
					}
				}
			}
			return true;
		}


		private void InitializeIcons()
		{
			foreach (DictionaryEntry  d in _icons)
			{
				Icon icon = (Icon)d.Value;
				icon._hasLinks = HasLinks(icon);
			}
		}

		private bool IconHasLinks(Icon icon)
		{
			return icon._hasLinks;
		}

		private bool HasLinks(Icon icon)
		{	
			foreach (DictionaryEntry  d in _icons)
			{
				Icon from = (Icon)d.Value;
				
				// does i point to this icon?
				for(int j=0; icon._pointsTo != null && j < icon._pointsTo.Length; ++j)
				{
					if (icon._pointsTo[j] == from.Name)
					{
						return true;
					}
				}

				// does this icon point to i?
				for(int j=0; from._pointsTo != null && j < from._pointsTo.Length; ++j)
				{
					if(_icons.Contains(from._pointsTo[j]))
					{
						Icon to = (Icon)_icons[from._pointsTo[j]];
						if(to.Name == icon.Name)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private ulong configurations(int locations, int icons)
		{
			ulong ans = 1;
			for(ulong i = (ulong)locations; i > (ulong)(locations - icons); --i)
			{	
				ulong temp = ans * i;
				// check for overflow:  max value of ulong is 18,446,744,073,709,551,615
				// temp should never be smaller than ans (unless we rollover) because 
				// temp always is ans * i, where i > 1.
				if (temp < ans) return 0;
				ans = temp;
			}
			return ans;
		}	
	}

	public class Icon
	{
		private string _name;
		private string _title;
		private string _url;
		private string _color;
		private int _height;
		private int _width;
		private int _size;
		private int _maxLabelLength;
		internal bool _hasLinks;
		internal Point _loc;
		internal string[] _pointsTo;
		internal ArrayList _lineSegments;
		internal void ClearLineSegments()
		{
			_lineSegments.Clear();
		}

		internal void AddLineSegment(LineSegment ls)
		{
			_lineSegments.Add(ls);
		}

		public Icon(
			string name, 
			string title, 
			string url, 
			string color, 
			int size, 
			int height, 
			int width, 
			int maxLabelLength, 
			string[] pointsTo)
		{
			_name = name;
			_title = title;
			_url = url;
			_color = color;
			_size = size;
			_height = height;
			_width = width;
			_pointsTo = pointsTo;
			_maxLabelLength = maxLabelLength;
			_lineSegments = new ArrayList();
			_loc = new Point(0,0);
			_hasLinks = false;
		}

		public void MoveTo(Point p)
		{
			// move the icon
			_loc = p;
			foreach (LineSegment ls in _lineSegments)
			{
				// move all links originating from this icon.
				ls.Start = p;
			}
		}

		public string GenerateHtmlString()
		{
			// shorten the name if necessary.
			string name = _name;
			if (name.Length > _maxLabelLength)
			{
				name = name.Substring(0, _maxLabelLength) + "...";
			}
			name = name.Replace("&","&amp;").Replace("<","&lt;").Replace(">","&gt;");
			
			string title = _title.Replace("&","&amp;").Replace("<","&lt;").Replace(">","&gt;");
			
			string str = "";
			if(_url.Length > 0)
			{
				str += "<a href='" + _url + "'>";
			}
			str += "<v:shape xmlns:v='urn:schemas-microsoft-com:vml' ";
			if(_title.Length > 0) 
			{
				str += "title='" + title + "' ";
			}
			str += "fillcolor='"+_color+"' strokecolor='"+_color+"' ";
			str += "style='position:absolute;cursor:hand;z-index:100;behavior:url(#default#VML);";
			str += "top:" + _loc.Y + "px;left:" + _loc.X + "px;width:"+_width+"px;height:"+_height+"px' ";
			str += "path='m "+_size+","+_size+" l "+_size+",-"+_size+", -"+_size+",-"+_size+", -"+_size+","+_size+", "+_size+","+_size+" xe'>";
			str += "<v:fill method='linear sigma' angle='-135' type='gradient' style='behavior:url(#default#VML);' />";
			str += "<span title='"+title+"' style='position:relative;top:"+(_size+5)+";left:0;cursor:hand;font-family:verdana;font-size:10;z-index:10;background-color:white;'>"+name+"</span>";
			str += "</v:shape>";
			if(_url.Length > 0)
			{
				str += "</a>";
			}
			foreach (LineSegment ls in _lineSegments)
			{
				str += ls.GenerateHtmlString();
			}
			return str;
		}

		public string Color
		{
			get 
			{
				return _color; 
			}
			set 
			{
				_color = value; 
			}
		}

		public string Url
		{
			get 
			{
				return _url; 
			}
			set 
			{
				_url = value; 
			}
		}

		public string Name
		{
			get 
			{
				return _name; 
			}
			set 
			{
				_name = value; 
			}
		}
	}

	public class LineSegment
	{
		private Point _pStart;
		private Point _pEnd;
		private double _slope;
		private bool _isVertical;
		private double _yIntercept;
		private string _title;
		private string _from;
		private string _to;
		private double _length;

		public LineSegment(Point startPoint, Point endPoint, string title, string fromPoint, string toPoint)
		{
			_pStart = startPoint;
			_pEnd = endPoint;
			_title = title;
			_from = fromPoint;
			_to = toPoint;
			Refresh();
		}

		public Point Start
		{
            get
            {
                return _pStart;
            }
            set
            {
                _pStart = value;
                Refresh();
            }
		}

		public Point End
		{
            get
            {
                return _pEnd;
            }
            set
            {
                _pEnd = value;
                Refresh();
            }
		}

		private void Refresh()
		{
			_isVertical = _pEnd.X - _pStart.X == 0 ? true : false;
			_slope = (_isVertical) ? 0 : (_pEnd.Y - _pStart.Y)/(_pEnd.X - _pStart.X);
			_yIntercept = (_isVertical) ? 0 : (_pEnd.X * _pStart.Y - _pStart.X * _pEnd.Y)/(_pEnd.X - _pStart.X);
			_length = Math.Sqrt(Math.Pow(_pEnd.X - _pStart.X,2) + Math.Pow(_pEnd.Y - _pStart.Y,2));
		}

		public double MaxY()
		{
			return _pStart.Y > _pEnd.Y ? _pStart.Y : _pEnd.Y;
		}

		public double MinY()
		{
			return _pStart.Y < _pEnd.Y ? _pStart.Y : _pEnd.Y;
		}

		public double MaxX()
		{
			return _pStart.X > _pEnd.X ? _pStart.X : _pEnd.X;
		}

		public double MinX()
		{
			return _pStart.X < _pEnd.X ? _pStart.X : _pEnd.X;
		}

		public bool Contains(Point p)
		{
			// if the segment is vertical and the point is contained in it...
			if(_isVertical && p.X == MaxX() && p.Y < MaxY() && p.Y > MinY())
			{
				return true;
			}

				// segment has zero slope and point is contained in it.
			else if(_slope == 0 && p.Y == MaxY() && p.X < MaxX() && p.X > MinX())
			{
				return true;
			}

				// segment has non-zero, non-infinite slope, and point is contained in it.
			else if(!_isVertical && p.Y == _slope * p.X + _yIntercept && 
					p.X < MaxX() && p.X > MinX() && p.Y < MaxY() && p.Y > MinY())
			{
				return true;
			}
			return false;
		}

		public bool Overlaps(LineSegment ls)
		{
			// if both are vertical, they cross if they have the same x value 
			// and they are overlapping in y.
			if(ls.IsVertical && _isVertical && ls.MaxX() == MaxX() &&
				((MinY() < ls.MinY() && MaxY() > ls.MinY()) ||
				(ls.MinY() < MinY() && ls.MaxY() > MinY())))
			{
				return true;
			}
			
			// if they are not vertical, and have the same slope, 
			// and have the same y intercept, then they cross if the segments are overlapping.
			else if(!ls.IsVertical && 
				!_isVertical && 
				_slope == ls._slope && 
				_yIntercept == ls._yIntercept &&
				((MinX() < ls.MinX() && MaxX() > ls.MinX()) ||
				(ls.MinX() < MinX() && ls.MaxX() > MinX())))
			{
				return true;
			}
			return false;
		}

		public bool Crosses(LineSegment ls)
		{
			// two special cases when one or the other is vertical.
			if(ls.IsVertical)
			{
				double cx = ls.MaxX();
				double cy = _slope*cx + _yIntercept;
				if(cy < ls.MaxY() && cy > ls.MinY())
				{
					return true;
				}
			}

			else if(_isVertical)
			{
				double cx = MaxX();
				double cy = ls.Slope*cx + ls.YIntercept;
				if(cy < MaxY() && cy > MinY())
				{
					return true;
				}
			}

			// we have two non parallel segments.  see if they cross.
			else if(!ls.IsVertical && !_isVertical)
			{
				double cx = (ls.YIntercept - _yIntercept)/(_slope - ls.Slope);
				double cy = (_slope*ls.YIntercept - ls.Slope*_yIntercept)/(_slope - ls.Slope);
				if(_slope != 0 && ls._slope != 0 &&
					cx < MaxX() && cx > MinX() && cy < MaxY() && cy > MinY() && 
					cx < ls.MaxX() && cx > ls.MinX() && cy < ls.MaxY() && cy > ls.MinY())
				{
					return true;
				}
				else if(_slope != 0 && ls._slope == 0 &&
					cx < MaxX() && cx > MinX() && cy < MaxY() && cy > MinY() && 
					cx < ls.MaxX() && cx > ls.MinX() && cy == ls.MaxY())
				{
					return true;
				}
				else if(_slope == 0 && ls._slope != 0 &&
					cx < MaxX() && cx > MinX() && cy == MaxY() && 
					cx < ls.MaxX() && cx > ls.MinX() && cy < ls.MaxY() && cy > ls.MinY())
				{
					return true;
				}
			} 
			return false;
		}

		public string From
		{
			get 
			{
				return _from; 
			}
		}

		public string To
		{
			get 
			{
				return _to; 
			}
		}

		public bool IsVertical
		{
			get 
			{
				return _isVertical; 
			}
		}

		public double Length
		{
			get 
			{
				return _length; 
			}
		}

		public double Slope
		{
			get 
			{
				return _slope; 
			}
		}

		public double YIntercept
		{
			get 
			{
				return _yIntercept; 
			}
		}

		public string GenerateHtmlString()
		{
			string s = "<v:shape ";
			if(_title.Length > 0) 
			{
				s +="title='" + _title.Replace("&","&amp;").Replace("<","&lt;").Replace(">","&gt;") + "' ";
			}
			s += " path='m " + _pStart.X + "," + _pStart.Y + " l " + _pEnd.X + "," + _pEnd.Y;
			s += " xe' strokecolor='gray' strokeweight='1px' style='position:absolute;behavior:url(#default#VML);z-index:-1000;height:1000;width:1000;top:0;left:0;'></v:shape>";
			return s;
		}
	}

	public class Point
	{
		private double _x;
		private double _y;
		
		public Point(double x, double y)
		{
			_x = x;
			_y = y;
		}

		public double X
		{
			get 
			{
				return _x; 
			}
			set 
			{
				_x = value; 
			}
		}

		public double Y
		{
			get 
			{
				return _y; 
			}
			set 
			{
				_y = value; 
			}
		}
	}
}
