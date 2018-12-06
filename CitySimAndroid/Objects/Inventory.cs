using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace CitySimAndroid.Objects
{
    public class Inventory : ObjectTracking, INotifyPropertyChanged
    {

        public static int ResourceMax = 500;

        public int Gold
        {
            get => _gold;
            set { _gold = value > ResourceMax ? ResourceMax : value; }
        }
        public int Wood
        {
            get => _wood;
            set { _wood = value > ResourceMax ? ResourceMax : value; }
        }
        public int Coal
        {
            get => _coal;
            set { _coal = value > ResourceMax ? ResourceMax : value; }
        }
        public int Iron
        {
            get => _iron;
            set { _iron = value > ResourceMax ? ResourceMax : value; }
        }
        public int Stone
        {
            get => _stone;
            set { _stone = value > ResourceMax ? ResourceMax : value; }
        }
        public int Workers
        {
            get => _workers;
            set { _workers = value > ResourceMax ? ResourceMax : value; }
        }
        public int Energy
        {
            get => _energy;
            set { _energy = value > ResourceMax ? ResourceMax : value; }
        }
        public int Food
        {
            get => _food;
            set { _food = value > ResourceMax ? ResourceMax : value; }
        }

        private int _gold;
        private int _wood;
        private int _coal;
        private int _iron;
        private int _stone;
        private int _workers;
        private int _energy;
        private int _food;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        protected void OnPropertyChanged(string propName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propName));
        }

        public Inventory()
        {
            Initialize();
            //ResetInventoryToBase();
        }

        // from debugging
        public void SetStone(int i, object sender, string function_name)
        {
            Log.Info("CitySim",  $"{sender.GetType().Name} called func. {function_name} to set stone to {i}");
            Stone = i;
        }

        public void ResetInventoryToBase()
        {
            Gold = 0;
            Wood = 0;
            Coal = 0;
            Iron = 0;
            SetStone(0, this, "ResetInventoryToBase");
            Workers = 0;
            Energy = 0;
            Food = 0;
        }

        public bool RemoveResource(string resource, int amount_requested)
        {
            if (amount_requested.Equals(0)) return true;

            try
            {
                if (string.IsNullOrEmpty(resource))
                    throw new NotSupportedException("Resource name cannot be null or empty.");

                if (amount_requested <= 0)
                    throw new NotSupportedException("Cannot request a resource amount equal or less than zero.");

                // switch based on resource name
                // try and subtract amount requested from resource
                // return true on success, false otherwise
                switch (resource.ToLower())
                {
                    case "gold":
                        Gold -= amount_requested;
                        return true;
                    case "wood":
                        Wood -= amount_requested;
                        return true;
                    case "coal":
                        Coal -= amount_requested;
                        return true;
                    case "stone":
                        Stone -= amount_requested;
                        return true;
                    case "iron":
                        Iron -= amount_requested;
                        return true;
                    case "workers":
                        Workers -= amount_requested;
                        return true;
                    case "energy":
                        Energy -= amount_requested;
                        return true;
                    case "food":
                        Food -= amount_requested;
                        return true;
                    default:
                        return false;
                }
            }
            catch (Exception e)
            {
                Log.Info("CitySim",  "Error getting resource: " + e.Message);
                return false;
            }
        }

        public bool RequestResource(string resource, int amount_requested)
        {
            if (amount_requested.Equals(0)) return true;

            try
            {
                if (string.IsNullOrEmpty(resource))
                    throw new NotSupportedException("Resource name cannot be null or empty.");

                if (amount_requested <= 0)
                    throw new NotSupportedException("Cannot request a resource amount equal or less than zero.");

                // switch based on resource name
                // try and subtract amount requested from resource
                // return true on success, false otherwise
                switch (resource.ToLower())
                {
                    case "gold":
                        if (amount_requested <= Gold)
                        {
                            Gold -= amount_requested;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case "wood":
                        if (amount_requested <= Wood)
                        {
                            Wood -= amount_requested;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case "coal":
                        if (amount_requested <= Coal)
                        {
                            Coal -= amount_requested;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case "iron":
                        if (amount_requested <= Iron)
                        {
                            Iron -= amount_requested;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case "stone":
                        if (amount_requested <= Stone)
                        {
                            Stone -= amount_requested;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case "workers":
                        if (amount_requested <= Workers)
                        {
                            Workers -= amount_requested;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case "energy":
                        if (amount_requested <= Energy)
                        {
                            Energy -= amount_requested;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case "food":
                        if (amount_requested <= Food)
                        {
                            Food -= amount_requested;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    default:
                        return false;
                }
            }
            catch (Exception e)
            {
                Log.Info("CitySim",  "Error getting resource: " + e.Message);
                return false;
            }
        }

        public bool AddResource(string resource, int amount)
        {
            if (amount.Equals(0)) return true;

            try
            {
                if (string.IsNullOrEmpty(resource))
                    throw new NotSupportedException("Resource name cannot be null or empty.");

                if (amount > ResourceMax)
                    throw new NotSupportedException("Cannot add a resource amount larger than max resources");

                // switch based on resource name
                // try and subtract amount requested from resource
                // return true on success, false otherwise
                switch (resource.ToLower())
                {
                    case "gold":
                        if (amount + Gold <= ResourceMax)
                        {
                            Gold += amount;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case "wood":
                        if (amount + Wood <= ResourceMax)
                        {
                            Wood += amount;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case "coal":
                        if (amount + Coal <= ResourceMax)
                        {
                            Coal += amount;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case "iron":
                        if (amount + Iron <= ResourceMax)
                        {
                            Iron += amount;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case "stone":
                        if (amount + Stone <= ResourceMax)
                        {
                            Stone += amount;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case "workers":
                        if (amount + Workers <= ResourceMax)
                        {
                            Workers += amount;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case "energy":
                        if (amount + Energy <= ResourceMax)
                        {
                            Energy += amount;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case "food":
                        if (amount + Food <= ResourceMax)
                        {
                            Food += amount;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    default:
                        return false;
                }
            }
            catch (Exception e)
            {
                Log.Info("CitySim",  "Error adding resource: " + e.Message);
                return false;
            }
        }
    }
}