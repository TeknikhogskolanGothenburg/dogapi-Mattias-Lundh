using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using test2.Models;

namespace dogapi_Mattias_Lundh.Controllers
{

    [Route("[controller]")]
    public class Dogs2Controller : Controller
    {
        // GET Dogs/
        [HttpGet]
        public IEnumerable<string> Get()
        {
            Response.ContentType = "application/json";
            return GetAllDogs().Select(d => d.BreedName).ToArray();
        }

        // GET Dogs/[breedName]
        [HttpGet("{id}")]
        public string Get(string id)
        {
            Response.ContentType = "application/json";
            if (GetAllDogs().Select(d => d.BreedName).Contains(id))
            {
                var file = System.IO.Directory.GetFiles("DogFiles", id + ".json");
                return System.IO.File.ReadAllText(file[0]);
            }
            Response.StatusCode = 400;
            return "no dog named " + id + " on server";

        }

        // POST Dogs/[breedName]
        [HttpPost]
        public void Post([FromBody]Dog dog)
        {
            string path = Environment.CurrentDirectory + "\\DogFiles\\" + dog.BreedName + ".json";
            if (!System.IO.File.Exists(path))
            {
                System.IO.File.WriteAllText(path, JsonConvert.SerializeObject(dog));
            }
            else
            {
                Response.StatusCode = 409;
            }
        }

        // PUT Dogs/[breedName]
        [HttpPut("{id}")]
        public void Put(string id, [FromBody]Dog dog)
        {
            string path = Environment.CurrentDirectory + "\\DogFiles\\" + id + ".json";
            //if(System.IO.Directory.GetFiles("DogFiles", "*.json").Select(s => s.Contains(id)).First())
            //{

            //}
            if (GetAllDogs().Select(d => d.BreedName).Contains(id))
            {
                Dog targetDog = GetAllDogs().Select(d => d).Where(d => d.BreedName == id).First();
                if (dog.BreedName != null)
                {
                    targetDog.BreedName = dog.BreedName;
                }
                if (dog.Description != null)
                {
                    targetDog.Description = dog.Description;
                }
                if (dog.WikipediaUrl != null)
                {
                    targetDog.WikipediaUrl = dog.WikipediaUrl;
                }
                System.IO.File.WriteAllText(path, JsonConvert.SerializeObject(targetDog));
            }

            else
            {
                Response.StatusCode = 400;
            }

        }

        // DELETE Dogs/[breedName]
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            if (GetAllDogs().Select(d => d.BreedName).Contains(id))
            {
                string path = Environment.CurrentDirectory + "\\DogFiles\\" + id + ".json";
                System.IO.File.Delete(path);
            }
            else
            {
                Response.StatusCode = 404;
            }
        }

        // DELETE Dogs/
        [HttpDelete]
        public void Delete()
        {
            if(GetAllDogs().Count != 0)
            {
               foreach(string file in System.IO.Directory.GetFiles("DogFiles", "*.json"))
                {
                    System.IO.File.Delete(file);
                }
            }
            else
            {
                Response.StatusCode = 404;
            }
        }

        private List<Dog> GetAllDogs()
        {
            var files = System.IO.Directory.GetFiles("DogFiles", "*.json");
            List<Dog> dogs = new List<Dog>();
            foreach (var file in files)
            {
                dogs.Add(JsonConvert.DeserializeObject<Dog>(System.IO.File.ReadAllText(file)));
            }
            return dogs;
        }

    }

}