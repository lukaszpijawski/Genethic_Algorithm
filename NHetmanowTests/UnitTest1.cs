using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GeneticAlgorithms;
using System.Collections.Generic;
using System.Linq;

namespace NHetmanowTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void DefaultConstructorTestMethod()
        {
            //arrange
            NHetmanow nHetmanowTesting = new NHetmanow();
            PrivateObject nHetmanow = new PrivateObject(nHetmanowTesting);
            PrivateType nHetmanowClass = new PrivateType(typeof(NHetmanow));
            //act
            var liczbaHetmanow = nHetmanowClass.GetStaticField("_liczbaHetmanow");
            var domyslnyRozmiarPopulacji = nHetmanow.GetProperty("RozmiarPopulacji");
            var domyslnePrawdopodobienstwoMutacji = nHetmanow.GetProperty("PrawdopodobienstwoMutacji");
            //assert
            Assert.AreEqual(8, liczbaHetmanow);
            Assert.AreEqual(10, domyslnyRozmiarPopulacji);
            Assert.AreEqual(0.01f, domyslnePrawdopodobienstwoMutacji);
        }

        [TestMethod]
        public void ZwrocTabliceNumerowKolumnTestMethod()
        {
            //arrange
            int size = 8;
            List<byte> expectedColumnsNumbers = new List<byte>()
            {
                1, 2, 3, 4, 5, 6, 7, 8
            };
            //act
            PrivateObject nHetmanow = new PrivateObject(new NHetmanow());
            var resultList = (List <byte> )nHetmanow.Invoke("ZwrocTabliceNumerowKolumn", size);
            //assert
            CollectionAssert.AreEqual(expectedColumnsNumbers, resultList);
        }

        [TestMethod]
        public void CzySaNaPrzekatnejTestMethod()
        {
            //arrange
            PrivateObject nHetmanow = new PrivateObject(new NHetmanow());
            Point<byte> numberToComapareWith = new Point<byte> { X = 2, Y = 4 };

            Point<byte> numberOnDiagonalLowerLeftToRight = new Point<byte> { X = 3, Y = 5 };
            Point<byte> numberOnDiagonalHigherLeftToRight = new Point<byte> { X = 1, Y = 3 };
            Point<byte> numberOnDiagonalLowerRightToLeft = new Point<byte> { X = 4, Y = 2 };
            Point<byte> numberOnDiagonalHigherRightToLeft = new Point<byte> { X = 1, Y = 5};

            Point<byte> numberDifferentByX = new Point<byte> { X = 0, Y = 3 };
            Point<byte> numberDifferentByY = new Point<byte> { X = 3, Y = 4 };
            Point<byte> numberDifferentByXAndY = new Point<byte> { X = 4, Y = 3 };
            //act
            var OnDiagonalLowerLeftToRight = nHetmanow.Invoke("CzySaNaPrzekatnej", numberToComapareWith, numberOnDiagonalLowerLeftToRight);
            var OnDiagonalHigherLeftToRight = nHetmanow.Invoke("CzySaNaPrzekatnej", numberToComapareWith, numberOnDiagonalHigherLeftToRight);
            var OnDiagonalLowerRightToLeft = nHetmanow.Invoke("CzySaNaPrzekatnej", numberToComapareWith, numberOnDiagonalLowerRightToLeft);
            var OnDiagonalHigherRightToLeft = nHetmanow.Invoke("CzySaNaPrzekatnej", numberToComapareWith, numberOnDiagonalHigherRightToLeft);

            var differentByX = nHetmanow.Invoke("CzySaNaPrzekatnej", numberToComapareWith, numberDifferentByX);
            var differentByY = nHetmanow.Invoke("CzySaNaPrzekatnej", numberToComapareWith, numberDifferentByY);
            var differentByXAndY = nHetmanow.Invoke("CzySaNaPrzekatnej", numberToComapareWith, numberDifferentByXAndY);
            //assert
            Assert.AreEqual(true, OnDiagonalLowerLeftToRight);
            Assert.AreEqual(true, OnDiagonalHigherLeftToRight);
            Assert.AreEqual(true, OnDiagonalLowerRightToLeft);
            Assert.AreEqual(true, OnDiagonalHigherRightToLeft);

            Assert.AreEqual(false, differentByX);
            Assert.AreEqual(false, differentByY);
            Assert.AreEqual(false, differentByXAndY);
        }

        [TestMethod]
        public void CzySaWTejSamejLiniiTestMethod()
        {
            //arrange
            PrivateObject nHetmanow = new PrivateObject(new NHetmanow());
            Point<byte> numberToCompareWith = new Point<byte> { X = 2, Y = 3 };

            Point<byte> numberInTheSamePlace = new Point<byte> { X = 2, Y = 3 };
            Point<byte> numberInTheSameColumn = new Point<byte> { X = 5, Y = 3 };
            Point<byte> numberInTheSameRow = new Point<byte> { X = 2, Y = 6 };

            Point<byte> numberInDifferentLines = new Point<byte> { X = 1, Y = 4 };
            //act
            var inTheSamePlace = nHetmanow.Invoke("CzySaWTejSamejLinii", numberToCompareWith, numberInTheSamePlace);
            var inTheSameColumn = nHetmanow.Invoke("CzySaWTejSamejLinii", numberToCompareWith, numberInTheSameColumn);
            var inTheSameRow = nHetmanow.Invoke("CzySaWTejSamejLinii", numberToCompareWith, numberInTheSameRow);
            var inDifferentLines = nHetmanow.Invoke("CzySaWTejSamejLinii", numberToCompareWith, numberInDifferentLines);
            //assert
            Assert.AreEqual(true, inTheSamePlace);
            Assert.AreEqual(true, inTheSameColumn);
            Assert.AreEqual(true, inTheSameRow);
            Assert.AreEqual(false, inDifferentLines);
        }

        [TestMethod]
        public void StworzTabliceSzachowanPoPrzekatnejTestMethod()
        {
            //arrange
            int rozmiar = 3;
            PrivateObject nHetmanow = new PrivateObject(new NHetmanow());
            //act
            var lista = (List<Point<byte>>[,])nHetmanow.Invoke("StworzTabliceSzachowanPoPrzekatnej", rozmiar);
            var oczekiwanaLista = new List<Point<byte>>
            {
                new Point<byte>{ X = 0, Y = 0 },
                new Point<byte>{ X = 1, Y = 1 },
                new Point<byte>{ X = 2, Y = 2 },
                //new Point<byte>{ X = 0, Y = 2 },
                //new Point<byte>{ X = 2, Y = 0 }
            };
            //assert
            var el = lista[1, 1];
            CollectionAssert.AreEqual(oczekiwanaLista, el);
        }             
    }
}
