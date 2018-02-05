using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dogapi_Mattias_Lundh.Controllers
{
    [Route("[controller]")]
    public class DogsController : Controller
    {
        // GET Dogs/
        [HttpGet]
        public IEnumerable<string> Get()
        {
            Response.ContentType = "application/json";
            return GetDogs();
        }

        // GET Dogs/[breedName]
        [HttpGet("{id}")]
        public List<string> Get(string id)
        {
            Response.ContentType = "application/json";
            return GetDog(id);
        }

        // POST Dogs/[breedName]
        [HttpPost]
        public void Post(IFormCollection collection)
        {
            CreateDog(collection);
        }

        // PUT Dogs/[breedName]
        [HttpPut("{id}")]
        public void Put(string id, IFormCollection collection)
        {
            UpdateDog(id, collection);
        }

        // DELETE Dogs/[breedName]
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            DeleteDog(id);
        }

        // DELETE Dogs/
        [HttpDelete]
        public void Delete()
        {
            DeleteDogs();
        }

        private void DeleteDogs()
        {
            List<string> dogs = FindAllFilePaths(@"/DogFiles/");
            if(dogs.Count == 0)
            {
                Response.StatusCode = 400;
            }
            foreach (string dogFilePath in dogs)
            {
                System.IO.File.Delete(dogFilePath);
            }
        }

        private void DeleteDog(string id)
        {
            if (System.IO.File.Exists(Environment.CurrentDirectory + @"/DogFiles/" + id + @".json"))
            {
                System.IO.File.Delete(Environment.CurrentDirectory + @"/DogFiles/" + id + @".json");
            }
            else
            {
                Response.StatusCode = 400;
            }
        }

        private void UpdateDog(string id, IFormCollection collection)
        {
            if (System.IO.File.Exists(Environment.CurrentDirectory + @"/DogFiles/" + id + @".json"))
            {
                List<string> targetDog = System.IO.File.ReadAllLines(Environment.CurrentDirectory + @"/DogFiles/" + id + @".json").ToList();
                if (collection.ContainsKey("BreedName"))
                {
                    targetDog[1] = "\"breedName\":\"" + collection["BreedName"] + "\", ";
                }
                if (collection.ContainsKey("WikipediaUrl"))
                {
                    targetDog[2] = "\"wikipediaUrl\":\"" + collection["WikipediaUrl"] + "\", ";
                }
                if (collection.ContainsKey("Description"))
                {
                    targetDog[3] = "\"description\":\"" + collection["Description"] + "\" ";
                }
                System.IO.File.Delete(Environment.CurrentDirectory + @"/DogFiles/" + id + @".json");
                System.IO.File.WriteAllLines(Environment.CurrentDirectory + @"/DogFiles/" + id + @".json", targetDog);
            }
            else
            {
                Response.StatusCode = 400;
            }
        }

        private void CreateDog(IFormCollection collection)
        {
            if (collection["BreedName"] != "" && collection["WikipediaUrl"] != "" && collection["Description"] != "")
            {
                if (!System.IO.File.Exists(Environment.CurrentDirectory + @"/DogFiles/" + collection["BreedName"] + @".json"))
                {
                    List<string> newDog = new List<string> {
                     "{",
                     "\"breedName\":\""+ collection["BreedName"] +"\", ",   
                     "\"wikipediaUrl\":\"" + collection["WikipediaUrl"] + "\", ",
                     "\"description\":\"" + collection["Description"] +"\" ",
                     "}"};
                    System.IO.File.WriteAllLines(Environment.CurrentDirectory + @"/DogFiles/" + collection["BreedName"] + @".json", newDog);
                }
                else
                {
                    Response.StatusCode = 409;
                }
            }
            else
            {
                Response.StatusCode = 400;
            }
        }

        private IEnumerable<string> GetDogs()
        {
            List<string> dogPaths = FindAllFilePaths(@"/DogFiles/");
            List<List<string>> dogs = new List<List<string>>();
            foreach (string path in dogPaths)
            {
                List<string> dog = new List<string>();
                dog.AddRange(System.IO.File.ReadAllLines(path));
                dogs.Add(dog);
            }

            return dogs.Select(d => d[1].Substring(d[1].IndexOf("breedName") + 12, d[1].Length - 3 - (d[1].Substring(0, d[1].IndexOf("breedName") + 12).Length)));
        }

        private List<string> GetDog(string request)
        {
            List<string> dogPaths = FindAllFilePaths(@"/DogFiles/");
            Dictionary<string, List<string>> dogs = new Dictionary<string, List<string>>();
            foreach (string path in dogPaths)
            {
                List<string> dog = new List<string>();
                dog.AddRange(System.IO.File.ReadAllLines(path));
                int index = path.IndexOf(@"DogFiles/") + 9;
                dogs.Add(path.Substring(index, path.Length - index - 5), dog);
            }
            if (dogs.ContainsKey(request))
            {
                return dogs[request];
            }
            Response.StatusCode = 400;
            return new List<string>();
        }

        private List<string> FindAllFilePaths(string dir)
        {
            List<string> result = new List<string>();
            foreach (string entity in System.IO.Directory.GetFileSystemEntries(Environment.CurrentDirectory + dir, "*"))
            {
                if (entity.Substring(entity.Length - 5, 5).Contains("."))
                {
                    result.Add(entity);
                }
                else
                {
                    foreach (string s in FindAllFilePaths(entity))
                    {
                        result.Add(s);
                    }
                }
            }
            return result;
        }

    }
}
