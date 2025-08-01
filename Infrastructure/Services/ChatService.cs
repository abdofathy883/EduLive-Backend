﻿using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services
{
    public class ChatService : IChatService
    {
        private readonly IGenericRepo<Message> messageRepo;
        private readonly IGenericRepo<Course> courseRepo;
        private readonly UserManager<BaseUser> userManager;

        public ChatService(IGenericRepo<Message> _messageRepo,
            IGenericRepo<Course> _courseRepo,
            UserManager<BaseUser> _usermanager)
        {
            messageRepo = _messageRepo;
            courseRepo = _courseRepo;
            userManager = _usermanager;
        }
        public async Task<IEnumerable<BaseUser>> GetAvailableInstructorsAsync(string studentId)
        {
            var purchasedCourses = await courseRepo
            .FindAsync(c => c.EnrolledStudents.Any(s => s.Id == studentId));

            return purchasedCourses
                .Select(c => c.Instructor)
                .Distinct();
        }

        public async Task<IEnumerable<Message>> GetConversationAsync(string user1Id, string user2Id)
        {
            return await messageRepo
            .FindAsync(m =>
                (m.SenderId == user1Id && m.ReceiverId == user2Id) ||
                (m.SenderId == user2Id && m.ReceiverId == user1Id)
            );
        }

        public async Task SendMessageAsync(string senderId, string receiverId, string messageContent)
        {
            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = messageContent
            };

            await messageRepo.AddAsync(message);
        }
    }
}
