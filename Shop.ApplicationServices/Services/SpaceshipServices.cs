using Microsoft.EntityFrameworkCore;
using Shop.Core.Domain;
using Shop.Core.Dtos;
using Shop.Core.ServiceInterface;
using Shop.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Shop.ApplicationServices.Services
{
    public class SpaceshipServices : ISpaceshipService
    {
        private readonly ShopDbContext _context;


        public SpaceshipServices
            (
            ShopDbContext context
            )
        {
            _context = context;
        }

        public async Task<Spaceship> Add(SpaceshipDto dto)
        {
            Spaceship spaceship = new Spaceship();
            FileToDatabase file = new FileToDatabase();

            spaceship.Id = Guid.NewGuid();
            spaceship.Name = dto.Name;
            spaceship.Model = dto.Model;
            spaceship.Company = dto.Company;
            spaceship.EnginePower = dto.EnginePower;
            spaceship.Country = dto.Country;
            spaceship.LaunchDate = dto.LaunchDate;
            spaceship.CreatedAt = DateTime.Now;
            spaceship.ModifiedAt = DateTime.Now;

            if (dto.Files != null)
            {
                file.ImageData = UploadFile(dto, spaceship);
            }


            await _context.Spaceship.AddAsync(spaceship);
            await _context.SaveChangesAsync();

            return spaceship;
        }


        public async Task<Spaceship> Delete(Guid id)
        {
            var spaceshipId = await _context.Spaceship
                .FirstOrDefaultAsync(x => x.Id == id);

            _context.Spaceship.Remove(spaceshipId);
            await _context.SaveChangesAsync();

            return spaceshipId;
        }


        public async Task<Spaceship> Update(SpaceshipDto dto)
        {
            Spaceship spaceship = new Spaceship();
            FileToDatabase file = new FileToDatabase();

            spaceship.Id = dto.Id;
            spaceship.Name = dto.Name;
            spaceship.Model = dto.Model;
            spaceship.Company = dto.Company;
            spaceship.EnginePower = dto.EnginePower;
            spaceship.Country = dto.Country;
            spaceship.LaunchDate = dto.LaunchDate;
            spaceship.CreatedAt = dto.CreatedAt;
            spaceship.ModifiedAt = DateTime.Now;

            if (dto.Files != null)
            {
                file.ImageData = UploadFile(dto, spaceship);
            }


            _context.Spaceship.Update(spaceship);
            await _context.SaveChangesAsync();
            return spaceship;
        }

        public async Task<Spaceship> GetAsync(Guid id)
        {
            var result = await _context.Spaceship
                .FirstOrDefaultAsync(x => x.Id == id);

            return result;
        }

        public byte[] UploadFile(SpaceshipDto dto, Spaceship spaceship)
        {
            if (dto.Files != null && dto.Files.Count > 0)
            {
                foreach (var photo in dto.Files)
                {
                    using (var target = new MemoryStream ())
                    {
                        FileToDatabase objFiles = new FileToDatabase
                        {
                            Id = Guid.NewGuid(),
                            ImageTitle = photo.FileName,
                            SpaceShipId = spaceship.Id
                        };

                        photo.CopyTo(target);
                        objFiles.ImageData = target.ToArray();

                        _context.FileToDatabase.Add(objFiles);
                    }
                }
            }

            return null;
        }

        public async Task<FileToDatabase> RemoveImage(FileToDatabaseDto dto)
        {
            var image = await _context.FileToDatabase
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

                _context.FileToDatabase.Remove(image);
            await _context.SaveChangesAsync();

            return image;
        }
    }
}