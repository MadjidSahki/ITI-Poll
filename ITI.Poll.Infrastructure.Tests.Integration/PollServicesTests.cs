using FluentAssertions;
using ITI.Poll.Model;
using ITI.Poll.Tests;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITI.Poll.Infrastructure.Tests.Integration
{
    public class PollServicesTests
    {
        [Test]
        public async Task create_poll()
        {
            using (PollContext pollContext = TestHelpers.CreatePollContext())
            {

                PollContextAccessor pollContextAccessor = new PollContextAccessor(pollContext);
                var pollRepository = new PollRepository(pollContextAccessor);
                var userRepository = new UserRepository(pollContextAccessor);


                string email = $"test-{Guid.NewGuid()}@test.fr";
                string nickname = $"Test-{Guid.NewGuid()}";

                Result<User> user = await TestHelpers.UserService.CreateUser(userRepository, email, nickname, "validpassword");
                Result<User> guest = await TestHelpers.UserService.CreateUser(userRepository, $"{email}-guest", $"{nickname}-guest", "validpassword");
                var pollDto = new NewPollDto
                {
                    AuthorId = user.Value.UserId,
                    Question = "Test-Question ",
                    GuestNicknames = new[] { guest.Value.Nickname },
                    Proposals = new[] { "proposal1", "proposal2" }
                };
                var pollCreated = await TestHelpers.PollService.CreatePoll(pollContext, pollRepository, userRepository, pollDto);

                pollCreated.IsSuccess.Should().BeTrue();
                pollCreated.Value.Guests.Should().HaveCount(pollDto.GuestNicknames.Length);
                pollCreated.Value.Proposals.Should().HaveCount(pollDto.Proposals.Length);
                pollCreated.Value.AuthorId.Should().Be(pollDto.AuthorId);
                pollCreated.Value.Question.Should().Be(pollDto.Question);

                var poll = await TestHelpers.PollService.FindById(pollRepository, pollCreated.Value.PollId);
                poll.IsSuccess.Should().BeTrue();

                await TestHelpers.PollService.DeletePoll(pollContext, pollRepository, pollCreated.Value.PollId);
                await TestHelpers.UserService.DeleteUser(pollContext, userRepository, pollRepository, user.Value.UserId);
                await TestHelpers.UserService.DeleteUser(pollContext, userRepository, pollRepository, guest.Value.UserId);



            }
        }

        [Test]
        public async Task remove_guest()
        {
            using (PollContext pollContext = TestHelpers.CreatePollContext())
            {

                PollContextAccessor pollContextAccessor = new PollContextAccessor(pollContext);
                var pollRepository = new PollRepository(pollContextAccessor);
                var userRepository = new UserRepository(pollContextAccessor);


                string email = $"test-{Guid.NewGuid()}@test.fr";
                string nickname = $"Test-{Guid.NewGuid()}";

                Result<User> user = await TestHelpers.UserService.CreateUser(userRepository, email, nickname, "validpassword");
                Result<User> guest2 = await TestHelpers.UserService.CreateUser(userRepository, $"{email}-guest2", $"{nickname}-guest2", "validpassword");
                Result<User> guest = await TestHelpers.UserService.CreateUser(userRepository, $"{email}-guest", $"{nickname}-guest", "validpassword");
                var pollDto = new NewPollDto
                {
                    AuthorId = user.Value.UserId,
                    Question = "Test-Question ",
                    GuestNicknames = new[] { guest.Value.Nickname, guest2.Value.Nickname },
                    Proposals = new[] { "proposal1", "proposal2" },
                };
                var pollCreated = await TestHelpers.PollService.CreatePoll(pollContext, pollRepository, userRepository, pollDto);

                var remove_guest = await TestHelpers.PollService.DeleteGuest(pollRepository,guest2.Value.UserId, pollCreated.Value.PollId);

                await TestHelpers.PollService.DeletePoll(pollContext, pollRepository, pollCreated.Value.PollId);
                await TestHelpers.UserService.DeleteUser(pollContext, userRepository, pollRepository, user.Value.UserId);
                await TestHelpers.UserService.DeleteUser(pollContext, userRepository, pollRepository, guest.Value.UserId);
                await TestHelpers.UserService.DeleteUser(pollContext, userRepository, pollRepository, guest2.Value.UserId);
            }
        }

    }
}
