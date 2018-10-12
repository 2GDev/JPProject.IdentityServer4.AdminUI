﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Jp.Application.Interfaces;
using Jp.Application.ViewModels;
using Jp.Application.ViewModels.RoleViewModels;
using Jp.Application.ViewModels.UserViewModels;
using Jp.Domain.Core.Bus;
using Jp.Domain.Core.Notifications;
using Jp.Domain.Interfaces;
using Jp.Infra.CrossCutting.Tools.Model;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jp.Management.Controllers
{
    [Route("[controller]"), Authorize(Policy = "ReadOnly")]
    public class UserAdminController : ApiController
    {
        private readonly IUserManageAppService _userManageAppService;
        private readonly ISystemUser _user;

        public UserAdminController(
            INotificationHandler<DomainNotification> notifications, 
            IMediatorHandler mediator,
            IUserManageAppService userManageAppService,
            ISystemUser user,
            IRoleManagerAppService roleManagerAppService) : base(notifications, mediator)
        {
            _userManageAppService = userManageAppService;
            _user = user;
        }


        [HttpGet, Route("list")]
        public async Task<ActionResult<DefaultResponse<IEnumerable<UserListViewModel>>>> List()
        {
            var irs = await _userManageAppService.GetUsers();
            return Response(irs);
        }

        [HttpGet, Route("details")]
        public async Task<ActionResult<DefaultResponse<UserViewModel>>> Details(string username)
        {
            var irs = await _userManageAppService.GetUserDetails(username);
            return Response(irs);
        }


        [HttpPost, Route("update"), Authorize(Policy = "Admin")]
        public async Task<ActionResult<DefaultResponse<bool>>> Update([FromBody] UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyModelStateErrors();
                return Response(false);
            }
            await _userManageAppService.UpdateUser(model);
            return Response(true);
        }


        [Route("remove-account"), HttpPost, Authorize(Policy = "Admin")]
        public async Task<ActionResult<DefaultResponse<bool>>> RemoveAccount([FromBody] RemoveAccountViewModel model)
        {
            await _userManageAppService.RemoveAccount(model);
            return Response(true);
        }


        [HttpGet, Route("claims")]
        public async Task<ActionResult<DefaultResponse<IEnumerable<ClaimViewModel>>>> Claims(string userName)
        {
            var clients = await _userManageAppService.GetClaims(userName);
            return Response(clients);
        }

        [HttpPost, Route("remove-claim"), Authorize(Policy = "Admin")]
        public async Task<ActionResult<DefaultResponse<bool>>> RemoveClaim([FromBody] RemoveUserClaimViewModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyModelStateErrors();
                return Response(false);
            }
            await _userManageAppService.RemoveClaim(model);
            return Response(true);
        }


        [HttpPost, Route("save-claim"), Authorize(Policy = "Admin")]
        public async Task<ActionResult<DefaultResponse<bool>>> SaveClaim([FromBody] SaveUserClaimViewModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyModelStateErrors();
                return Response(false);
            }
            await _userManageAppService.SaveClaim(model);
            return Response(true);
        }

        [HttpGet, Route("roles")]
        public async Task<ActionResult<DefaultResponse<IEnumerable<RoleViewModel>>>> Roles(string userName)
        {
            var clients = await _userManageAppService.GetRoles(userName);
            return Response(clients);
        }

        [HttpPost, Route("remove-role"), Authorize(Policy = "Admin")]
        public async Task<ActionResult<DefaultResponse<bool>>> RemoveRole([FromBody] RemoveUserRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyModelStateErrors();
                return Response(false);
            }

            model.UserId = _user.UserId;
            await _userManageAppService.RemoveRole(model);
            return Response(true);
        }

        [HttpPost, Route("save-role"), Authorize(Policy = "Admin")]
        public async Task<ActionResult<DefaultResponse<bool>>> SaveRole([FromBody] SaveUserRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyModelStateErrors();
                return Response(false);
            }
            await _userManageAppService.SaveRole(model);
            return Response(true);
        }

        [HttpGet, Route("logins")]
        public async Task<ActionResult<DefaultResponse<IEnumerable<UserLoginViewModel>>>> Logins(string userName)
        {
            var clients = await _userManageAppService.GetLogins(userName);
            return Response(clients);
        }

        [HttpGet, Route("remove-login")]
        public async Task<ActionResult<DefaultResponse<bool>>> RemoveLogin([FromBody] RemoveUserLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyModelStateErrors();
                return Response(false);
            }
            await _userManageAppService.RemoveLogin(model);
            return Response(true);
        }

        [HttpGet, Route("users-from-role")]
        public async Task<ActionResult<DefaultResponse<IEnumerable<UserListViewModel>>>> UsersFromRole(string[] role)
        {
            var clients = await _userManageAppService.GetUsersInRole(role);
            return Response(clients);
        }

    }
}