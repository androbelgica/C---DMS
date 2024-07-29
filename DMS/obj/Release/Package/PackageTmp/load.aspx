<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="load.aspx.cs" Inherits="DMS.load" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8"/>
    <meta http-equiv="X-UA-Compatible" content="IE=edge"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>Loading...</title>
    <style>
        @import url('https://fonts.googleapis.com/css2?family=Maven+Pro:wght@400..900&display=swap');
        @import url('https://fonts.googleapis.com/css2?family=DM+Serif+Text:ital@0;1&display=swap');

        body {
            background-color: #191818;
        }

        .loader {
            width: 65px;
            aspect-ratio: 1;
            --_g: no-repeat radial-gradient(farthest-side, rgba(180, 140, 44, 0.7) 100%, rgba(180, 140, 44, 0));
            background: var(--_g) 0 0, var(--_g) 100% 0, var(--_g) 100% 100%, var(--_g) 0 100%;
            background-size: 50% 50%;
            animation: l38 .5s infinite;
        }

        @keyframes l38 {
            100% {
                background-position: 100% 0, 100% 100%, 0 100%, 0 0
            }
        }

        .content {
            position: absolute;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            text-align: center;
        }

        .line-1 {
            font-family: Maven Pro;
            font-size: 15px;
            letter-spacing: 2px;
            margin-top: 10px;
            margin-bottom: 20px;
            color: #717171;
        }

        .line-2 {
            font-family: DM Serif Text;
            font-size: 35px;
            color: #fff;
        }

        @media only screen and (max-width: 461px) {
            
            .loader {
                width: 65px;
            }

            .line-1 {
                font-size: 13px;
            }

            .line-2 {
                font-size: 25px;
            }

            .content {
                width: 100%;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="content">
            <div class="loader"></div>
            <br />
            <div class="line-1">THE BELLEVUE MANILA</div>
            <div class="line-2">
                Always at Home in
                <br />
                World-Class Hospitality
            </div>
        </div>
    </form>
</body>
</html>
