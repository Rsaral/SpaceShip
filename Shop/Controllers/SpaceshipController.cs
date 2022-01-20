using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.ApplicationServices.Services;
using Shop.Core.Dtos;
using Shop.Core.ServiceInterface;
using Shop.Data;
using Shop.Models.Spaceship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Controllers
{
    public class SpaceshipController : Controller
    {
        private readonly ShopDbContext _context;
        private readonly ISpaceshipService _spaceshipService;

        public SpaceshipController
            (
                ShopDbContext context,
                ISpaceshipService spaceshipServices
            )
        {
            _context = context;
            _spaceshipService = spaceshipServices;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var result = _context.Spaceship
                .OrderByDescending(y => y.CreatedAt)
                .Select(x => new SpaceshipListViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Company = x.Company,
                    Country = x.Country,
                    Model = x.Model,
                    EnginePower = x.EnginePower,
                    LaunchDate = x.LaunchDate,
                    CreatedAt = x.CreatedAt,
                    ModifiedAt = x.ModifiedAt

                });

            return View(result);
        }

        [HttpGet]
        public IActionResult Add()
        {
            SpaceshipViewModel spaceship = new SpaceshipViewModel();

            return View("Edit", spaceship);
        }

        [HttpPost]
        public async Task<IActionResult> Add(SpaceshipViewModel vm)
        {
            var dto = new SpaceshipDto()
            {
                Id = vm.Id,
                Name = vm.Name,
                Company = vm.Company,
                Country = vm.Country,
                Model = vm.Model,
                EnginePower = vm.EnginePower,
                LaunchDate = vm.LaunchDate,
                CreatedAt = vm.CreatedAt,
                ModifiedAt = vm.ModifiedAt,
                Image = vm.Image.Select(x => new FileToDatabaseDto
                {
                    Id = x.Id,
                    ImageData = x.ImageData,
                    ImageTitle = x.ImageTitle,
                    SpaceShipId = x.SpaceShipId

                }).ToArray()



                
            };

            var result = await _spaceshipService.Add(dto);

            if (result == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index), vm);
        }


        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await _spaceshipService.Delete(id);
            if (product == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var spaceship = await _spaceshipService.GetAsync(id);

            if (spaceship == null)
            {
                return NotFound();
            }

            var photos = await _context.FileToDatabase
                .Where(x => x.SpaceShipId == id)
                .Select(y => new ImageViewModel
                {
                    ImageData = y.ImageData,
                    Id = y.Id,
                    Image = string.Format("data:image/gif;based64,(0)", Convert.ToBase64String(y.ImageData)),
                    ImageTitle = y.ImageTitle,
                    SpaceShipId = y.Id,

                }).ToArrayAsync();
            
                
                var vm = new SpaceshipViewModel();

            vm.Id = spaceship.Id;
            vm.Name = spaceship.Name;
            vm.Model = spaceship.Model;
            vm.Company = spaceship.Company;
            vm.EnginePower = spaceship.EnginePower;
            vm.Country = spaceship.Country;
            vm.LaunchDate = spaceship.LaunchDate;
            vm.ModifiedAt = spaceship.ModifiedAt;
            vm.CreatedAt = spaceship.CreatedAt;
            vm.Image.AddRange(photos);

            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(SpaceshipViewModel vm)
        {
            var dto = new SpaceshipDto()
            {
                Id = vm.Id,
                Name = vm.Name,
                Model = vm.Model,
                Company = vm.Company,
                EnginePower = vm.EnginePower,
                Country = vm.Country,
                LaunchDate = vm.LaunchDate,
                CreatedAt = vm.CreatedAt,
                ModifiedAt = vm.ModifiedAt,
                Files = vm.Files,
                Image = vm.Image.Select(x => new FileToDatabaseDto
                {
                    Id = x.Id,
                    ImageData = x.ImageData,
                    ImageTitle = x.ImageTitle,
                    SpaceShipId = x.SpaceShipId
                })
            };

            var result = await _spaceshipService.Update(dto);

            if (result == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index), vm);

            
        }

        [HttpPost]
        public async Task<IActionResult> RemoveImage(ImageViewModel file)
        {
            var dto = new FileToDatabaseDto()
            {
                Id = file.Id
            };
            var image = await _spaceshipService.RemoveImage(dto);
            if (image == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
