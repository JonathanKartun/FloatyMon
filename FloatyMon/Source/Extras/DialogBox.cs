using Android.App;
using Android.Views;
using System.Threading.Tasks;
using Xamarin.Essentials;
using static Android.Content.Res.Resources;

public class Show_Dialog
{
    public enum MessageResult
    {
        NONE = 0,
        OK = 1,
        CANCEL = 2,
        ABORT = 3,
        RETRY = 4,
        IGNORE = 5,
        YES = 6,
        NO = 7
    }

    Activity mcontext;
    public Show_Dialog(Activity activity) : base()
    {
        this.mcontext = activity;
    }

    public Task<MessageResult> ShowDialog(string Title, string Message, bool SetCancelable = false, MessageResult PositiveButton = MessageResult.OK, MessageResult NegativeButton = MessageResult.NONE, MessageResult NeutralButton = MessageResult.NONE, int IconAttribute = Android.Resource.Attribute.AlertDialogIcon)
    {
        var tcs = new TaskCompletionSource<MessageResult>();

        Theme appTheme = mcontext.ApplicationContext.Theme;
        var builder = new AlertDialog.Builder(new ContextThemeWrapper(mcontext, appTheme));

        builder.SetIconAttribute(IconAttribute);
        builder.SetTitle(Title);
        builder.SetMessage(Message);
        builder.SetCancelable(SetCancelable);

        builder.SetPositiveButton((PositiveButton != MessageResult.NONE) ? PositiveButton.ToString() : string.Empty, (senderAlert, args) =>
        {
            tcs.SetResult(PositiveButton);
        });
        builder.SetNegativeButton((NegativeButton != MessageResult.NONE) ? NegativeButton.ToString() : string.Empty, delegate
        {
            tcs.SetResult(NegativeButton);
        });
        builder.SetNeutralButton((NeutralButton != MessageResult.NONE) ? NeutralButton.ToString() : string.Empty, delegate
        {
            tcs.SetResult(NeutralButton);
        });

        MainThread.BeginInvokeOnMainThread(() =>
        {
            builder.Show();
        });

        return tcs.Task;
    }
}
