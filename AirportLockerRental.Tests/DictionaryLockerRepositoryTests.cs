﻿using AirportLockerRental.UI.DTOs;
using AirportLockerRental.UI.Storage;
using NUnit.Framework;

namespace AirportLockerRental.Tests
{
    public class DictionaryLockerRepositoryTests
    {
        private ILockerRepository _repo;

        [SetUp]
        public void Initialize()
        {
            _repo = new DictionaryLockerRepository(10);

            // add a few sample records
            _repo.Add(new LockerContents
            {
                LockerNumber = 1,
                UserName = "Testy Tester",
                Description = "Things and Stuff"
            });

            _repo.Add(new LockerContents
            {
                LockerNumber = 7,
                UserName = "Data Sampler",
                Description = "Bits & Bytes"
            });
        }

        [Test]
        public void CanGetContents()
        {
            Assert.That(_repo.Get(1), !Is.Null);
        }

        [Test]
        public void EmptyLockerReturnsNull()
        {
            Assert.That(_repo.Get(4), Is.Null);
        }

        [Test]
        public void AvailabilityCheck()
        {
            Assert.That(_repo.IsAvailable(4), Is.True);

            Assert.That(_repo.IsAvailable(1), Is.False);
        }

        [Test]
        public void CannotAddOverCapacity()
        {
            var small = new DictionaryLockerRepository(1);

            Assert.That(small.Add(new LockerContents
            {
                LockerNumber = 1,
                UserName = "Testy Tester",
                Description = "Things and Stuff"
            }), Is.True);

            Assert.That(small.Add(new LockerContents
            {
                LockerNumber = 2,
                UserName = "Stacked Overflow",
                Description = "Too many things"
            }), Is.False);
        }

        [Test]
        public void CannotAddOccupied()
        {
            Assert.That(_repo.Add(new LockerContents
            {
                LockerNumber = 1,
                UserName = "Baddy McUnavailableFace",
                Description = "Forbidden items"
            }), Is.False);
        }

        [Test]
        public void CanRemoveItem()
        {
            Assert.That(_repo.Remove(7), !Is.Null);
        }

        [Test]
        public void RemoveEmptyReturnsNull()
        {
            Assert.That(_repo.Remove(10), Is.Null);
        }
    }
}
