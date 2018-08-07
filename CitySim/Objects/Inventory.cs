using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CitySim.Objects
{
    public class Inventory
    {
        public static int ResourceMax = 99999;
        public int Gold { get; set; } = 500;
        public int Wood { get; set; } = 100;
        public int Coal { get; set; } = 0;
        public int Iron { get; set; } = 0;
        public int Workers { get; set; } = 50;
        public int Energy { get; set; } = 20;
        public int Food { get; set; } = 10;

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
                Console.WriteLine("Error getting resource: " + e.Message);
                return false;
            }
        }

        public bool RequestResource(string resource, int amount_requested)
        {
            if (amount_requested.Equals(0)) return true;

            try
            {
                if(string.IsNullOrEmpty(resource))
                    throw new NotSupportedException("Resource name cannot be null or empty.");

                if(amount_requested <= 0)
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
                Console.WriteLine("Error getting resource: " + e.Message);
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
                Console.WriteLine("Error adding resource: " + e.Message);
                return false;
            }
        }
    }
}
