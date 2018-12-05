using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace CitySimMobile.Objects
{
    public abstract class ObjectTracking
    {
        private Dictionary<string, object> _originalValues = new Dictionary<string, object>();

        public void Initialize()
        {
            PropertyInfo[] props = this.GetType().GetProperties();

            // save current val of props to dictionary
            foreach (PropertyInfo prop in props)
            {
                this._originalValues.Add(prop.Name, prop.GetValue(this));
            }
        }

        public Dictionary<string, object> GetChanges()
        {
            PropertyInfo[] props = this.GetType().GetProperties();
            var latestChanges = new Dictionary<string, object>();

            // save current val of props to our dict
            foreach (PropertyInfo prop in props)
            {
                latestChanges.Add(prop.Name, prop.GetValue(this));
            }

            // get all props
            PropertyInfo[] tempProps = GetType().GetProperties().ToArray();

            // filter props by only getting what has changed
            props = tempProps.Where(p => !Equals(p.GetValue(this, null), this._originalValues[p.Name])).ToArray();

            foreach (PropertyInfo prop in props)
            {
                Console.WriteLine($"{prop.Name} changed to: {prop.GetValue(this)}");
                latestChanges.Add(prop.Name, prop.GetValue(this));
            }

            return latestChanges;
        }
    }
}