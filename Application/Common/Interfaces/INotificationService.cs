using Domain.Entities;


namespace Application.Common.Interfaces
{
    public interface INotificationService
    {
        bool AccountConfirmationNotify(UserEntity user, string confirmationToken);
        bool AccountActivationNotify(UserEntity user);
        bool GrantAccessNotify(UserEntity user, string claimId, bool needConfirm);
    }
}
