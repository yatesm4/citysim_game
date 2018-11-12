using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitySim.Objects
{
    public class Resident
    {
        // 200 random names for use with civilians
        public static readonly List<string> RandomNameList = new List<string>()
        {
            "Jaron Chandler","Bryant Lloyd","Harley Ponce","Madalyn Huff","Semaj Pineda","Madden Smith","Samir Ellison","Kristian Burke","Jovanny Haley","Natalie Mccullough","Sage Sanchez","Madeline Saunders","Savanna Pearson","Mylie Walton","Kendrick Snyder","Julianne Franco","Jazlynn Cowan","Aisha Morales","Eden Reese","Jaida Stanley","Randy Nichols","Elianna Lowery","Patrick Sweeney","Kali Schwartz","Hamza Parks","Rolando Simpson","Brielle Frost","Rosemary Shea","Luciana Velazquez","Lillie Cordova","German Browning","Pierce Pham","Brendon Wheeler","Kaley Osborne","Augustus Nolan","Lia Salazar","Elvis Combs","Kayden Casey","Chasity Bullock","Frances Mcdonald","Leon Calderon","Elise Chung","Jairo Kramer","Efrain Cain","Shawn Sullivan","Ariana Hanson","Piper Morrow","Marely Oneill","Casey Joseph","Jacoby Finley","Henry Newman","Jase Garner","Anna Moore","Rowan Hardin","Denisse Flynn","Liam Raymond","Gia Weaver","Willie Reynolds","Eliana Holt","Rayne Anthony","Derrick Harrison","Georgia Moyer","Tommy Ward","Mekhi Tucker","Izabelle Bean","Krish Mcneil","Harmony Ewing","Jacqueline Newton","Sidney Leon","Taylor Mullins","Everett Hurley","Alexia Mckenzie","Josiah Proctor","Alexander Shaw","Davian Cross","Karma Snow","Benjamin Johnston","Paisley Merritt","Emmy Horne","Karter Cummings","Natalia Oliver","Lea Goodman","Yurem Cannon","Nathalie Schaefer","Kadyn Tapia","Jeremiah Park","Keshawn Dean","Nia Colon","Jordan Cobb","Alyson Byrd","Andrew Gardner","Olive Young","Aria Marshall","Haley Schultz","Ashlynn Padilla","Alondra Wilson","Mckenzie Moses","Isaac Dillon","Avery Guerrero","Savion Cline","Abbey Bond","Arabella Taylor","Mareli Alexander","Alison Michael","John Hartman","Darwin Johns","Averi Myers","Amber Thomas","Caden Curtis","Siena Bowman","Tate Woodard","Estrella Owens","Cooper Ibarra","Grady Joyce","Cassandra Harrell","Karli Martin","Kelvin Gomez","Brynn Hutchinson","Bryson Osborn","Asher Lin","Giuliana Wiley","Oliver Meza","Myles Daniel","Kassidy Dunn","Kash Stephens","Terrell Carson","Daphne Rhodes","Raphael Swanson","Chandler Fowler","Delilah Blackwell","Hazel Deleon","Samson Bauer","Kaitlyn Jefferson","Carsen Hicks","Jaydon Moody","Dixie Gamble","Kaydence Briggs","Marcus Jimenez","Davin Waller","Rylan Acevedo","Nyasia Herring","Adolfo Lucero","Brock Faulkner","Frankie Richard","Nevaeh Benson","Gilbert Mendoza","Amya Hensley","Elena Brooks","Quinton Hooper","Salvatore Downs","Kailey Lyons","Jordan Oconnor","Saniyah Ryan","Jaylynn Leonard","Ireland Porter","Sydney Buck","Brian Summers","Titus Sexton","Meghan Cruz","Tristian Miranda","Justine Dodson","Dominique Perkins","Giovani Contreras","Kyra Todd","Kiera Robbins","Madelyn Tate","Kaden Arellano","Aileen Stuart","Xiomara Waters","Owen Olson","Ellis Allison","Javon Atkinson","Ty Keith","Sebastian Ochoa","Audrey Hodges","Sage Carney","Joaquin Schmitt","Tianna Knapp","Jakobe Allen","Anabel Gaines","Ryker Berger","Reilly Tyler","Avah White","Clare Cameron","Naomi Wolfe","Julian Baker","Reed Hancock","Marianna Peterson","Heather Mcgee","Kasen Costa","Anya Hayes","Rebekah Wise","Dayton Ramsey","Julio Sloan","Edgar Hunter","Isabela Stout","Richard Adams","Ally Chaney","Mariela Meyers","Nash Savage"
        };

        public string Name { get; set; } = "Jeff";

        public int DaysAlive { get; set; } = 0;

        public float Education { get; set; } = 0f;

        public float Health { get; set; } = 1.0f;

        public float Happiness { get; set; } = 0f;
    }
}
