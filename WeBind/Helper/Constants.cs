using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeBind.Helper
{
    public class Constants
    {
        public static string AdminEmailId = "connect@webind.in";

        public static int[] ExperienceYears = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50 };
        public static int[] ExperienceMonths = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
        
        public static string ReferAFriendDefaultMessgae = "Hi Please register to this web site and educate India...";
        public static string ReferAFriendSentMessage = "Thank You for helping our cause";

        public static string FeedbackSentMsg = "Thanks for the feedback !";
        public static string Thankyou = "Thank You !!!";
        public static string Sorry = "Sorry !!!";
        public static string Congratulations = "Congratulations !!!";
        public static string RequestAlreadyGenerated = "Your request has already been generated.";
        public static string RequestRejected = "Your request has been rejected.";
        public static string RequestAlreadyAccepted = "Your request has already been Accepted";
        public static string RequestGenerated = "Your request has been generated.";
        
        public static string ExpertProfilePicBasePath = "~/images/people/expertsPhoto";
        public static string ProfileDetailUpdatedMessage = "Thank You for updating your details.";
        
        public static string ExceptionErrorMailBody = "Hi Rushil,<br /><br /> Exception : {0}<br /> {1}";
        public static string ExceptionErrorMailSubject = "WeBind || Error";

        public static string Website = "http://webind.in/";

        public static string TwitterLink = "http://www.twitter.com/we_bind";
        public static string FacebookLink = "http://www.facebook.com/webindc2c";
        public static string WebindLink = "http://webind.in";
        public static string WebinMail = "mailto:connect@webind.in";

        public static string PL_ExpertName = "{ExpertName}";
        public static string PL_CampusName = "{CampusName}";
        public static string PL_CallBackUrl = "{CallBackUrl}";
        
        public static string PL_Twitter = "{Twitter}";
        public static string PL_Facebook = "{Facebook}";
        public static string PL_WebindLink = "{WebindLink}";
        public static string PL_WebindMail = "{WebindMail}";

        public static string PL_LatestSession = "{LatestSession}";
        public static string PL_LatestSession_link = Website + "Account/RegisterConfirmation";
        public static string PL_UpdateProfile = "{UpdateProfile}";
        public static string PL_UpdateProfile_link = Website + "Educator/EducatorProfile";
        public static string PL_RequestSession = "{RequestSession}";
        public static string PL_RequestSession_link = Website + "Educator/EducatorSessionRequest";
        public static string PL_SearchExpert = "{SearchExpert}";
        public static string PL_SearchExpert_link = Website + "Home/MastersList";
        public static string PL_SendMessage = "{SendMessage}";
        public static string PL_SendMessage_link = Website + "Educator/EducatorMessages";
        public static string PL_SendFeedback = "{SendFeedback}";
        public static string PL_SendFeedback_link = Website + "Educator/EducatorCampusFeedback";
        public static string PL_HostMCVideo = "{HostMCVideo}";
        public static string PL_HostMCVideo_link = Website + "Educator/EducatorCampusFeedback";
        public static string PL_JoinSessionVideo = "{JoinSessionVideo}";
        public static string PL_JoinSessionVideo_link = Website + "Educator/EducatorCampusFeedback";


        public static string PL_E_Feedback = "{Feedback}";
        public static string PL_E_Feedback_link = Website + "Expert/ExpertCampusFeedback";
        public static string PL_E_DashBoard = "{DashBoard}";
        public static string PL_E_DashBoard_link = Website + "Expert/ExpertDashBoard";
        public static string PL_E_Requests = "{Requests}";
        public static string PL_E_Requests_link = Website + "Expert/ExpertRequest";
        public static string PL_E_Messages = "{Messages}";
        public static string PL_E_Messages_link = Website + "Expert/ExpertMessages";
        public static string PL_E_Sessions = "{Sessions}";
        public static string PL_E_Sessions_link = Website + "Expert/ExpertWebinarsList";
        public static string PL_E_Profile = "{Profile}";
        public static string PL_E_Profile_link = Website + "Expert/ExpertProfile";
        public static string PL_E_ReferFriend = "{ReferFriend}";
        public static string PL_E_ReferFriend_link = Website + "Expert/ExpertReferFriend";
    }
}