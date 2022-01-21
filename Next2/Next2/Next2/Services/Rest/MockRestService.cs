using Next2.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace Next2.Services.Rest
{
    public class MockRestService : IRestService
    {
        private List<MemberModel> _members;

        #region -- IRestService implementation --

        public Task<T> RequestAsync<T>(HttpMethod httpMethod, string requestUrl, Dictionary<string, string> additionalHeaders = null, object requestBody = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region -- Private helpers --

        private void InitMembers()
        {
            _members = new List<MemberModel>
            {
                new MemberModel
                {
                    Id = 1,
                    CustomerName = "Martin Schleifer",
                    Phone = "732-902-8298",
                    MembershipStartTime = DateTime.ParseExact(
                        "Mar 28 2021 / 08:36 PM",
                        Constants.Values.MEMBERSHIP_TIME_FORMAT,
                        CultureInfo.InvariantCulture),
                    MembershipEndTime = DateTime.ParseExact(
                        "Mar 28 2021 / 08:36 PM",
                        Constants.Values.MEMBERSHIP_TIME_FORMAT,
                        CultureInfo.InvariantCulture),
                },
                new MemberModel
                {
                    Id = 2,
                    CustomerName = "Ashlynn Westervelt",
                    Phone = "599-663-3931",
                    MembershipStartTime = DateTime.ParseExact(
                        "Mar 28 2021 / 09:11 PM",
                        Constants.Values.MEMBERSHIP_TIME_FORMAT,
                        CultureInfo.InvariantCulture),
                    MembershipEndTime = DateTime.ParseExact(
                        "Mar 28 2021 / 09:11 PM",
                        Constants.Values.MEMBERSHIP_TIME_FORMAT,
                        CultureInfo.InvariantCulture),
                },
                new MemberModel
                {
                    Id = 3,
                    CustomerName = "Carla Dorwart",
                    Phone = "090-540-7412",
                    MembershipStartTime = DateTime.ParseExact(
                        "Mar 29 2021 / 09:30 AM",
                        Constants.Values.MEMBERSHIP_TIME_FORMAT,
                        CultureInfo.InvariantCulture),
                    MembershipEndTime = DateTime.ParseExact(
                        "Mar 29 2021 / 09:30 AM",
                        Constants.Values.MEMBERSHIP_TIME_FORMAT,
                        CultureInfo.InvariantCulture),
                },
                new MemberModel
                {
                    Id = 4,
                    CustomerName = "Davis Septimus",
                    Phone = "301-472-3355",
                    MembershipStartTime = DateTime.ParseExact(
                        "Mar 29 2021 / 09:22 AM",
                        Constants.Values.MEMBERSHIP_TIME_FORMAT,
                        CultureInfo.InvariantCulture),
                    MembershipEndTime = DateTime.ParseExact(
                        "Mar 29 2021 / 09:22 AM",
                        Constants.Values.MEMBERSHIP_TIME_FORMAT,
                        CultureInfo.InvariantCulture),
                },
                new MemberModel
                {
                    Id = 5,
                    CustomerName = "Kierra Bergson",
                    Phone = "503-778-7600",
                    MembershipStartTime = DateTime.ParseExact(
                        "Mar 29 2021 / 12:22 AM",
                        Constants.Values.MEMBERSHIP_TIME_FORMAT,
                        CultureInfo.InvariantCulture),
                    MembershipEndTime = DateTime.ParseExact(
                        "Mar 29 2022 / 12:22 AM",
                        Constants.Values.MEMBERSHIP_TIME_FORMAT,
                        CultureInfo.InvariantCulture),
                },
                new MemberModel
                {
                    Id = 6,
                    CustomerName = "Angel Dias",
                    Phone = "672-533-7711",
                    MembershipStartTime = DateTime.ParseExact(
                        "Mar 28 2021 / 08:54 AM",
                        Constants.Values.MEMBERSHIP_TIME_FORMAT,
                        CultureInfo.InvariantCulture),
                    MembershipEndTime = DateTime.ParseExact(
                        "Mar 28 2021 / 08:54 AM",
                        Constants.Values.MEMBERSHIP_TIME_FORMAT,
                        CultureInfo.InvariantCulture),
                },
                new MemberModel
                {
                    Id = 7,
                    CustomerName = "Kaiya Dorwart",
                    Phone = "688-905-0586",
                    MembershipStartTime = DateTime.ParseExact(
                        "Mar 29 2021 / 03:51 PM",
                        Constants.Values.MEMBERSHIP_TIME_FORMAT,
                        CultureInfo.InvariantCulture),
                    MembershipEndTime = DateTime.ParseExact(
                        "Mar 29 2021 / 03:51 PM",
                        Constants.Values.MEMBERSHIP_TIME_FORMAT,
                        CultureInfo.InvariantCulture),
                },
                new MemberModel
                {
                    Id = 8,
                    CustomerName = "Lincoln Lipshutz",
                    Phone = "174-449-2766",
                    MembershipStartTime = DateTime.ParseExact(
                        "Mar 29 2021 /02:48 AM",
                        Constants.Values.MEMBERSHIP_TIME_FORMAT,
                        CultureInfo.InvariantCulture),
                    MembershipEndTime = DateTime.ParseExact(
                        "Mar 29 2021 /02:48 AM",
                        Constants.Values.MEMBERSHIP_TIME_FORMAT,
                        CultureInfo.InvariantCulture),
                },
                new MemberModel
                {
                    Id = 9,
                    CustomerName = "Ann Schleifer",
                    Phone = "962-399-9765",
                    MembershipStartTime = DateTime.ParseExact(
                        "Mar 29 2021 / 02:07 AM",
                        Constants.Values.MEMBERSHIP_TIME_FORMAT,
                        CultureInfo.InvariantCulture),
                    MembershipEndTime = DateTime.ParseExact(
                        "Mar 29 2021 / 02:07 AM",
                        Constants.Values.MEMBERSHIP_TIME_FORMAT,
                        CultureInfo.InvariantCulture),
                },
                new MemberModel
                {
                    Id = 10,
                    CustomerName = "Randy Mango",
                    Phone = "500-803-7621",
                    MembershipStartTime = DateTime.ParseExact(
                        "Mar 29 2021 / 02:47 AM",
                        Constants.Values.MEMBERSHIP_TIME_FORMAT,
                        CultureInfo.InvariantCulture),
                    MembershipEndTime = DateTime.ParseExact(
                        "Mar 29 2021 / 02:47 AM",
                        Constants.Values.MEMBERSHIP_TIME_FORMAT,
                        CultureInfo.InvariantCulture),
                },
                new MemberModel
                {
                    Id = 11,
                    CustomerName = "Cheyenne Calzoni",
                    Phone = "576-273-4018",
                    MembershipStartTime = DateTime.ParseExact(
                        "Mar 29 2021 /20:36 AM",
                        Constants.Values.MEMBERSHIP_TIME_FORMAT,
                        CultureInfo.InvariantCulture),
                    MembershipEndTime = DateTime.ParseExact(
                        "Mar 29 2022 / 20:36 AM",
                        Constants.Values.MEMBERSHIP_TIME_FORMAT,
                        CultureInfo.InvariantCulture),
                },
                new MemberModel
                {
                    Id = 12,
                    CustomerName = "Zaire Levin",
                    Phone = "601-611-1754",
                    MembershipStartTime = DateTime.ParseExact(
                        "Mar 29 2021 / 21:11 AM",
                        Constants.Values.MEMBERSHIP_TIME_FORMAT,
                        CultureInfo.InvariantCulture),
                    MembershipEndTime = DateTime.ParseExact(
                        "Mar 29 2021 / 21:11 AM",
                        Constants.Values.MEMBERSHIP_TIME_FORMAT,
                        CultureInfo.InvariantCulture),
                },
                new MemberModel
                {
                    Id = 13,
                    CustomerName = "Carla Mango",
                    Phone = "142-826-7912",
                    MembershipStartTime = DateTime.ParseExact(
                        "Mar 29 2021 /  09:30 AM",
                        Constants.Values.MEMBERSHIP_TIME_FORMAT,
                        CultureInfo.InvariantCulture),
                    MembershipEndTime = DateTime.ParseExact(
                        "Mar 29 2021 /  09:30 AM",
                        Constants.Values.MEMBERSHIP_TIME_FORMAT,
                        CultureInfo.InvariantCulture),
                },
                new MemberModel
                {
                    Id = 14,
                    CustomerName = "Cheyenne Levin",
                    Phone = "210-626-0640",
                    MembershipStartTime = DateTime.ParseExact(
                        "Mar 29 2021 / 09:22 AM",
                        Constants.Values.MEMBERSHIP_TIME_FORMAT,
                        CultureInfo.InvariantCulture),
                    MembershipEndTime = DateTime.ParseExact(
                        "Mar 29 2021 / 09:22 AM",
                        Constants.Values.MEMBERSHIP_TIME_FORMAT,
                        CultureInfo.InvariantCulture),
                },
            };
        }

        #endregion
    }
}