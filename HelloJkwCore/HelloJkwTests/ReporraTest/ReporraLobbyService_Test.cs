﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HelloJkwService.Reporra;
using Xunit;

namespace HelloJkwTests.ReporraTest
{
    public class ReporraLobbyService_Test
    {
        private readonly IReporraLobbyService _lobby;

        public ReporraLobbyService_Test()
        {
            _lobby = new ReporraLobbyService();
        }

        [Fact]
        public void CreateRoom_Normal()
        {
            // Arrange
            var roomOption = new ReporraRoomOption
            {
                RoomName = "ABC",
            };

            // Act
            var roomResult = _lobby.CreateRoom(roomOption);

            // Assert
            Assert.True(roomResult.IsSuccess);
        }

        [Fact]
        public void CreateRoom_Duplicated()
        {
            // Arrange
            var roomOption = new ReporraRoomOption
            {
                RoomName = "ABC",
            };

            // Act
            _lobby.CreateRoom(roomOption);
            var roomResult2 = _lobby.CreateRoom(roomOption);

            // Assert
            Assert.Equal(ReporraError.DuplicatedName, roomResult2.FailReason);
        }

        [Fact]
        public void CreateRoomAndCheckList_Normal()
        {
            // Arrange
            var roomOption = new ReporraRoomOption
            {
                RoomName = "ABC",
            };

            // Act
            var room = _lobby.CreateRoom(roomOption).Result;

            var roomList = _lobby.GetRoomList();

            // Assert
            Assert.Contains(roomList, x => x == room);
        }

        [Fact]
        public void DeleteRoom_Normal()
        {
            // Arrange
            var roomOption = new ReporraRoomOption
            {
                RoomName = "ABC",
            };

            // Act
            var room = _lobby.CreateRoom(roomOption).Result;
            _lobby.DeleteRoom(roomOption.RoomName);

            var roomList = _lobby.GetRoomList();

            // Assert
            Assert.DoesNotContain(roomList, x => x == room);
        }

        [Fact]
        public void UserList_StartsEmpty()
        {
            var userList = _lobby.GetUserList();

            Assert.Empty(userList);
        }

        [Fact]
        public void UserList_EnterUser()
        {
            var user1 = new ReporraUser();
            _lobby.EnterUser(user1);

            Assert.Contains(_lobby.GetUserList(), x => x == user1);
        }

        [Fact]
        public void UserList_LeaveUser()
        {
            var user1 = new ReporraUser();
            _lobby.EnterUser(user1);
            _lobby.LeaveUser(user1);

            Assert.Empty(_lobby.GetUserList());
        }
    }
}