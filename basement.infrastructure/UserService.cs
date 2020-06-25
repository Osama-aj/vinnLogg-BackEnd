using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using basement.core.Entities.UserSection;
using basement.core.Models.UserModels;
using basement.infrastructure.Interfaces;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;

namespace basement.infrastructure
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IDatabaseBoundary _databaseBoundary;

        public UserService(IDatabaseBoundary databaseBoundary, IMapper mapper)
        {
            _databaseBoundary = databaseBoundary;
            _mapper = mapper;
        }


        public bool Create(AddUserModel model, ClaimsPrincipal user)
        {

            var uId = GetUserIdFromSession(user);

            var isExist = _databaseBoundary.GetUserById(uId);
            if (isExist != null)
                throw new ApplicationException("This User is already exists");

            //auto mapper from  RegisterModel to user entity and add the hashedPass and add it to db
            User userToAdd = _mapper.Map<AddUserModel, User>(model);
            userToAdd.UserId = uId;


            var isSucceed = _databaseBoundary.AddUser(userToAdd);
            if (isSucceed)
                throw new ApplicationException("Something went wrong");

            return true;


        }




        public bool Update(UpdateUserModel model, ClaimsPrincipal user)
        {
            var userId = GetUserIdFromSession(user);
            var targetUserToUpdate = _databaseBoundary.GetUserById(userId);

            if (targetUserToUpdate == null)
                throw new Exception("user with this user name not found!");

            _mapper.Map<UpdateUserModel, User>(model, targetUserToUpdate);

            var isSucceed = _databaseBoundary.UpdateUser(targetUserToUpdate);

            return isSucceed;


        }

        public bool Delete(ClaimsPrincipal user)
        {
            var userId = GetUserIdFromSession(user);
            var userToDelete = _databaseBoundary.GetUserById(userId);
            if (userToDelete == null)
                return false;

            var isSucceed = _databaseBoundary.DeleteUser(userToDelete);
            return isSucceed;
        }





        public string GetUserIdFromSession(ClaimsPrincipal user)
        {

            var claimsIdentity = user.Identity as ClaimsIdentity;
            if (claimsIdentity == null)
                throw new NullReferenceException("identity not recognized!");
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var userExist = CheckIfUserExistsInOurDb( userId);
            if (!userExist)
                return null;
            return userId;
        }

        private bool CheckIfUserExistsInOurDb( string userId)
        {
            // var user =await  FirebaseAuth.DefaultInstance.GetUserAsync(userId);
            var userIDb = _databaseBoundary.GetUserById(userId);
            if (userIDb != null)
                return true;

            return _databaseBoundary.AddUser(new User { UserId = userId, Username = "new user" });
        }











        //public async Task<string> GetUserIdFromFirebaseToken(string token)
        //{
        //    FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance
        //        .VerifyIdTokenAsync(token);
        //    string uid = decodedToken.Uid;

        //    Console.WriteLine(("----------------"));
        //    return uid;
        //}

    }
}