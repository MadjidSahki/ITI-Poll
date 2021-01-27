using FluentAssertions;
using ITI.Poll.Infrastructure;
using ITI.Poll.Tests;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITI.Poll.Model.Tests.Integration
{
    public class PollServiceTest
    {
        [Test]
        public async Task create_poll()
        {
            using (PollContext pollContext = TestHelpers.CreatePollContext())
            {
                PollContextAccessor pollContextAccessor = new PollContextAccessor(pollContext);
                PollRepository pollRepository = new PollRepository(pollContextAccessor);
                UserRepository userRepository = new UserRepository(pollContextAccessor);

                NewPollDto newPollDto = new NewPollDto
                {
                    AuthorId = 0,
                    Question = "Question?",
                };
                PollService sut = new PollService();

                Result<Poll> poll = await sut.CreatePoll(pollContext, pollRepository, userRepository, newPollDto);

                Result<Poll> expected = Result.CreateSuccess(new Poll(0, 0, "Question?", false));
                poll.Should().BeEquivalentTo(expected);
            }
        }
    }
}
