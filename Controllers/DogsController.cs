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
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            Response.ContentType = "application/json";
            return GetDogs().Select(d => d[1].Substring(d[1].IndexOf("breedName") + 12, d[1].Length - 3 - (d[1].Substring(0, d[1].IndexOf("breedName") + 12).Length)));
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public List<string> Get(string id)
        {
            Response.ContentType = "application/json";
            return GetDog(id);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]FormCollection collection)
        {
            if (collection["BreedName"] != "" && collection["WikipediaUrl"] != "" && collection["Description"] != "")
            {
                System.IO.File.Create(Environment.CurrentDirectory + @"/DogFiles/" + collection["BreedName"] + @".json");
            }
            else
            {
                Response.StatusCode = 400;
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private List<List<string>> GetDogs()
        {
            List<string> dogPaths = FindAllFilePaths(@"/DogFiles/");
            List<List<string>> dogs = new List<List<string>>();
            foreach (string path in dogPaths)
            {
                List<string> dog = new List<string>();
                dog.AddRange(System.IO.File.ReadAllLines(path));
                dogs.Add(dog);
            }
            return dogs;
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

            if (dogs[request] != null)
            {
                return dogs[request];
            }
            throw new Exception("invalid dog, no recource found");
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
