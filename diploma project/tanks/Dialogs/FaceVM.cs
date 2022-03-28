using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Reflection;
using tanks.Models;
using System.Collections;
using System.Diagnostics;
using System.Globalization;

namespace tanks.Dialogs
{
    public class GridCellVM
    {
        public int Row { get; set; }
        public int Column { get; set; }
        private string _Text;
        public bool IsModified;
        public string Text {
            get { return _Text; }
            set { _Text = value; IsModified = true; } }
        public bool IsValue { get; set; }
    }

    public class SheetVM
    {
        public string SheetName { get; set; }
        public ObservableCollection<GridCellVM> Cells { get; private set; }
        public int Rows { get;  set; }
        public int Columns { get; set; }

        public SheetVM()
        {
            Cells = new ObservableCollection<GridCellVM>();
        }
    }

    public class FaceVM :INotifyPropertyChanged
    {
        public string Title { get; set; }
        public ObservableCollection<SheetVM> Sheets { get; private set; }

        public FaceVM()
        {
            Sheets = new ObservableCollection<SheetVM>();

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Sheets"));
        }

        Models.ModelObject _mobj;

        public event PropertyChangedEventHandler PropertyChanged;

        public Models.ModelObject mobj
        {
            get { return _mobj; }
            set
            {
                _mobj = value;
                rebuildData();
            }
        }

        void rebuildData()
        {
            var unit1 = _mobj;
            Sheets.Clear();
            Title = unit1.Id;

            foreach (var propertyInfo in unit1.GetType().GetProperties())
            {
                var propertyAttribyte = propertyInfo.GetCustomAttribute<PropertyTypeAttribute>();
                if (propertyAttribyte == null) continue;

                var value2 = unit1.GetType().GetProperty(propertyInfo.Name).GetValue(unit1, null);

                if (propertyInfo.PropertyType.IsConstructedGenericType || propertyInfo.PropertyType.IsArray)
                {
                    var enumerable = (value2 as IEnumerable);
                    Debug.Assert(enumerable != null);
                    int idx = 0;
                    foreach (var en_item in enumerable)
                    {
                        var en_type = en_item.GetType();
                        Debug.Assert(en_type.IsPrimitive
                            || ((en_item as System.String) != null));
                        //Console.WriteLine("  {0}[{1}] = {2} <{3}>", propertyInfo.Name, idx, en_item, propertyAttribyte.type); 

                        var sheetName = propertyAttribyte.type.ToString();

                        SheetVM sheet;

                        var r=Sheets.Where((s) => s.SheetName.Equals(sheetName));

                        if (r.Count() == 0)
                        {
                            sheet = new SheetVM { SheetName = sheetName, Columns = 2, Rows = 0 };
                            Sheets.Add(sheet);
                        }
                        else
                            sheet = r.First();

                        GridCellVM c1 = new GridCellVM
                        {
                            Row = sheet.Rows,
                            Column = 0,
                            Text = String.Format("{0}[{1}]", propertyInfo.Name, idx),
                            IsValue = false
                        };
                        c1.IsModified = false;

                        sheet.Cells.Add(c1);

                        GridCellVM c2 = new GridCellVM
                        {
                            Row = sheet.Rows,
                            Column = 1,
                            Text = ToString(en_item),
                            IsValue = true
                        };
                        c2.IsModified = false;

                        sheet.Cells.Add(c2);
                        sheet.Rows++;
                        idx++;
                    }
                }
                else
                {
                    if (propertyInfo.PropertyType.IsEnum)
                    {

                    }
                    else
                    {

                        Debug.Assert(propertyInfo.PropertyType.IsPrimitive
                            || ((value2 as System.String) != null));
                    }
                    //Console.WriteLine("  {0} = {1} <{2}>", propertyInfo.Name, value2, propertyAttribyte.type);

                    var sheetName = propertyAttribyte.type.ToString();

                    SheetVM sheet;

                    var r = Sheets.Where((s) => s.SheetName.Equals(sheetName));

                    if (r.Count() == 0)
                    {
                        sheet = new SheetVM { SheetName = sheetName, Columns = 2, Rows = 0 };
                        Sheets.Add(sheet);
                    }
                    else
                        sheet = r.First();

                    GridCellVM c1 = new GridCellVM
                    {
                        Row = sheet.Rows,
                        Column = 0,
                        Text = propertyInfo.Name,
                        IsValue = false
                    };
                    c1.IsModified = false;

                    sheet.Cells.Add(c1);

                    GridCellVM c2 = new GridCellVM
                    {
                        Row = sheet.Rows,
                        Column = 1,
                        Text = ToString(value2),
                        IsValue = true
                    };
                    c2.IsModified = false;

                    sheet.Cells.Add(c2);
                    sheet.Rows++;
                }
            }
        }

        string ToString(object o)
        {
            string s="";

            if ((o as Int32?) != null)
            {
                Int32 v = (Int32)(o as Int32?);
                s = v.ToString("#0", CultureInfo.InvariantCulture);
            }
            else if ((o as Double?) != null)
            {
                Double v = (Double)(o as Double?);
                s = v.ToString("#0.0#####", CultureInfo.InvariantCulture);
            }
            else
            {
                s = o.ToString();
            }

            return s;
        }

        public void Commit()
        {
            var unit1 = _mobj;

            var r = Sheets.SelectMany(s => s.Cells);
            var r0 = r.Where(c => c.Column == 0);
            var r1 = r.Where(c => c.Column == 1);

            var rr = r0.Zip(r1,
                (c0, c1) => new Tuple<bool, string, string>(c1.IsModified, c0.Text, c1.Text))
                .Where(t => t.Item1).Select((t) => new Tuple<string, string>(t.Item2, t.Item3));

            foreach (var pp in rr)
            {
                var sp = pp.Item1.Split(new char[2] { '[',']'});

                var pr = unit1.GetType().GetProperty(sp[0]);

                if (sp.Length == 1)
                {
                    var ns = pr.PropertyType.Name;
                    try
                    {
                        switch (ns)
                        {
                            case "Double":
                                pr.SetValue(unit1, Double.Parse(pp.Item2, CultureInfo.InvariantCulture));
                                break;
                            case "EUMassOrVolume":
                            case "EUTime":
                            case "EULevel":
                            case "PIDDirection":
                            case "PIDMode":
                            case "EUPressure":
                            case "EUTemperature":
                                pr.SetValue(unit1, Enum.Parse(pr.PropertyType, pp.Item2));
                                break;
                            default:
                                Debug.Assert(false);
                                break;
                        }
                    }
                    catch (Exception e)
                    { 
                    }
                }
                else
                {
                    var col = pr.GetValue(unit1) as IList;

                    int idx = Int32.Parse(sp[1], CultureInfo.InvariantCulture);

                    var ns = col[idx].GetType().Name;
                    try
                    {
                        switch (ns)
                        {
                            case "Double":
                                col[idx] = Double.Parse(pp.Item2, CultureInfo.InvariantCulture);
                                break;
                            case "Int32":
                                col[idx] = Int32.Parse(pp.Item2, CultureInfo.InvariantCulture);
                                break;
                            default:
                                Debug.Assert(false);
                                break;
                        }
                    }
                    catch (Exception e)
                    { 
                    }
                }
            }
        }
    }
}
