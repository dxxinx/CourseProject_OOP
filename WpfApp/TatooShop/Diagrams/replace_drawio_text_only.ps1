$ErrorActionPreference = 'Stop'

function ConvertTo-XmlAttributeValue {
    param([string]$Value)
    return [System.Security.SecurityElement]::Escape($Value)
}

function Set-MxCellValue {
    param(
        [string]$Content,
        [string]$Id,
        [string]$Value
    )

    $escapedId = [regex]::Escape($Id)
    $escapedValue = ConvertTo-XmlAttributeValue $Value
    $pattern = '(<mxCell\b(?=[^>]*\bid="' + $escapedId + '")[^>]*\bvalue=")([^"]*)(")'
    return [regex]::Replace($Content, $pattern, '${1}' + $escapedValue + '${3}', 1)
}

function Update-DrawioText {
    param(
        [string]$Path,
        [hashtable]$Values
    )

    $content = Get-Content -LiteralPath $Path -Raw
    foreach ($id in $Values.Keys) {
        $content = Set-MxCellValue -Content $content -Id $id -Value $Values[$id]
    }
    Set-Content -LiteralPath $Path -Value $content -Encoding UTF8
}

$telegramDir = 'C:\Users\diana\Downloads\Telegram Desktop'
$classPath = Join-Path $telegramDir 'class_diagram.drawio'
$architecturePath = (Get-ChildItem -LiteralPath $telegramDir -Filter '*.drawio' |
    Where-Object { $_.Name -ne 'class_diagram.drawio' -and $_.Length -eq 19857 } |
    Select-Object -First 1 -ExpandProperty FullName)

if (-not $architecturePath) {
    throw 'Architecture drawio file was not found.'
}

$classValues = @{
    'title' = 'Class diagram of TatooShop application'
    '8rkKDdmEsx0Q5-RZuWgH-1' = 'Class diagram&nbsp;'
    '8rkKDdmEsx0Q5-RZuWgH-4' = '<div><b>Acc</b></div>'
    '8rkKDdmEsx0Q5-RZuWgH-5' = '+ Id: int<br>+ AccountType: AccountType<br>+ Login: string<br>+ Password: string<br>+ EMail: string<br>+ Phone: string<br>+ Surname: string<br>+ Name: string<br>+ MiddleName: string<br>+ FullName: string'
    '8rkKDdmEsx0Q5-RZuWgH-7' = '+ ForSearch(): string'
    '8rkKDdmEsx0Q5-RZuWgH-8' = '<div><b>Reservation</b></div>'
    '8rkKDdmEsx0Q5-RZuWgH-9' = '+ Id: int<br>+ User: User<br>+ Master: Master<br>+ Date: DateTime<br>+ Time: UserTime<br>+ Status: ReservationStatus<br>+ StatusText: string<br>+ CanCancel: bool<br>+ TimeText: string'
    '8rkKDdmEsx0Q5-RZuWgH-11' = '+ GetReservationDateTime(): DateTime'
    '8rkKDdmEsx0Q5-RZuWgH-12' = '<b style="background-color: transparent;">SupportRequest</b>'
    '8rkKDdmEsx0Q5-RZuWgH-13' = '+ Id: int<br>+ User: User<br>+ Subject: string<br>+ Message: string<br>+ AdminReply: string<br>+ CreatedAt: DateTime<br>+ ProcessedAt: DateTime?<br>+ IsProcessed: bool'
    '8rkKDdmEsx0Q5-RZuWgH-16' = '<div><b>InternalNotification</b></div>'
    '8rkKDdmEsx0Q5-RZuWgH-17' = '+ Id: int<br>+ User: User<br>+ Title: string<br>+ Message: string<br>+ CreatedAt: DateTime<br>+ IsRead: bool'
    '8rkKDdmEsx0Q5-RZuWgH-20' = '<div><b>Feedback</b></div>'
    '8rkKDdmEsx0Q5-RZuWgH-21' = '+ Id: int<br>+ User: User<br>+ Master: Master<br>+ Rating: int<br>+ Comment: string<br>+ CreatedAt: DateTime'
    '8rkKDdmEsx0Q5-RZuWgH-24' = '<div><b>Favourite</b></div>'
    '8rkKDdmEsx0Q5-RZuWgH-25' = '+ Id: int<br>+ User: User<br>+ Sketch: Sketch'
    '8rkKDdmEsx0Q5-RZuWgH-28' = '&lt;abstract&gt;<div><b>ViewModel</b></div>'
    '8rkKDdmEsx0Q5-RZuWgH-29' = '+ PropertyChanged: event<br># OnPropertyChanged(propertyName): void'
    '8rkKDdmEsx0Q5-RZuWgH-32' = '<div><b>Master</b></div>'
    '8rkKDdmEsx0Q5-RZuWgH-33' = '+ Id: int<br>+ Image: byte[]<br>+ Surname: string<br>+ Name: string<br>+ MiddleName: string<br>+ Type: TatooTypes<br>+ Experience: int'
    '8rkKDdmEsx0Q5-RZuWgH-36' = '<div><b>Sketch</b></div>'
    '8rkKDdmEsx0Q5-RZuWgH-37' = '+ Id: int<br>+ Type: TatooTypes<br>+ Placement: TattooPlacement<br>+ Size: TattooSize<br>+ Complexity: TattooComplexity<br>+ EstimatedHours: int<br>+ Image: byte[]'
    '8rkKDdmEsx0Q5-RZuWgH-40' = '&lt;enumeration&gt;<div><b>AccountType</b></div>'
    '8rkKDdmEsx0Q5-RZuWgH-41' = '+ Admin<br>+ User<br>+ None'
    '8rkKDdmEsx0Q5-RZuWgH-44' = '&lt;enumeration&gt;<div><b>ReservationStatus</b></div>'
    '8rkKDdmEsx0Q5-RZuWgH-45' = '+ Pending<br>+ Confirmed<br>+ Cancelled<br>+ Visited'
    '8rkKDdmEsx0Q5-RZuWgH-48' = '<div><b>DataConnection</b></div>'
    '8rkKDdmEsx0Q5-RZuWgH-49' = '+ Accounts: List<br>+ Masters: List<br>+ Sketch: List<br>+ Reservations: List<br>+ Feedbacks: List<br>+ Favourites: List<br>+ SupportRequests: List'
    '8rkKDdmEsx0Q5-RZuWgH-51' = '+ GetInstance(): DataConnection<br>+ SaveChanges(): int'
    '8rkKDdmEsx0Q5-RZuWgH-52' = '<div><b>Manager</b></div>'
    '8rkKDdmEsx0Q5-RZuWgH-53' = '+ CurrentUser: Acc<br>+ AccountType: AccountType'
    '8rkKDdmEsx0Q5-RZuWgH-55' = '+ Login(login, password): bool<br>+ Register(...): (bool, string)<br>+ HashPassword(password): string<br>+ Logout(): void'
    '8rkKDdmEsx0Q5-RZuWgH-56' = '<div><b>ReservationViewModel</b></div>'
    '8rkKDdmEsx0Q5-RZuWgH-57' = '+ SelectedMaster: Master'
    '8rkKDdmEsx0Q5-RZuWgH-59' = '+ MastersList: List<br>+ ReservationCommand: ICommand<br>+ RefreshAvailableTimes(): void'
    '8rkKDdmEsx0Q5-RZuWgH-60' = '<div><b>SketchViewModel</b></div>'
    '8rkKDdmEsx0Q5-RZuWgH-61' = '- _allSketches: List'
    '8rkKDdmEsx0Q5-RZuWgH-63' = '+ ApplyFilters(): void<br>+ RefreshSketches(): void<br>+ OpenPreview(sketch): void'
    '8rkKDdmEsx0Q5-RZuWgH-64' = '<div><b>AdminViewModel</b></div>'
    '8rkKDdmEsx0Q5-RZuWgH-65' = '- _db: DataConnection'
    '8rkKDdmEsx0Q5-RZuWgH-67' = '+ SaveSketch(): void<br>+ SaveMaster(): void<br>+ ConfirmReservation(): void<br>+ ProcessSupportRequest(): void'
    '8rkKDdmEsx0Q5-RZuWgH-68' = '<div><b>NotificationService</b></div>'
    '8rkKDdmEsx0Q5-RZuWgH-69' = '+ NotifyReservationConfirmed(...)<br>+ HasUnreadNotifications(...): bool'
    '8rkKDdmEsx0Q5-RZuWgH-71' = '+ MarkAllAsRead(...): void'
}

$architectureValues = @{
    'ouVfGpyxBDxrH55nrl9G-150' = '.NET 8 WPF (Windows Desktop)'
    'ouVfGpyxBDxrH55nrl9G-151' = 'WPF application TatooShop'
    'ouVfGpyxBDxrH55nrl9G-152' = 'UI (Presentation Layer)'
    'ouVfGpyxBDxrH55nrl9G-153' = 'LogWindow.xaml'
    'ouVfGpyxBDxrH55nrl9G-154' = 'PersonalWindow.xaml'
    'ouVfGpyxBDxrH55nrl9G-155' = 'RegistrationWindow.xaml'
    'ouVfGpyxBDxrH55nrl9G-156' = 'FavoriteWindow.xaml'
    'ouVfGpyxBDxrH55nrl9G-157' = 'StartWindow.xaml'
    'ouVfGpyxBDxrH55nrl9G-158' = 'App.xaml'
    'ouVfGpyxBDxrH55nrl9G-159' = 'AdminWindow.xaml'
    'ouVfGpyxBDxrH55nrl9G-160' = 'MastersWindow.xaml'
    'ouVfGpyxBDxrH55nrl9G-161' = 'ReservationWindow.xaml'
    'ouVfGpyxBDxrH55nrl9G-162' = 'SketchWindow.xaml'
    'ouVfGpyxBDxrH55nrl9G-163' = 'ImagePreviewWindow.xaml'
    'ouVfGpyxBDxrH55nrl9G-164' = 'StartWindow.xaml.cs'
    'ouVfGpyxBDxrH55nrl9G-166' = 'Business Logic'
    'ouVfGpyxBDxrH55nrl9G-167' = '<span style="text-align: left;">ViewModels</span>'
    'ouVfGpyxBDxrH55nrl9G-199' = 'StartViewModel<br style="">LogViewModel<br style="">RegistrationViewModel<br style="">SketchViewModel<br style="">MastersViewModel<br style="">ReservationViewModel<br style="">PersonalViewModel<br style="">AdminViewModel'
    'ouVfGpyxBDxrH55nrl9G-168' = '<span style="text-align: left;">Services / Utilities</span>'
    'ouVfGpyxBDxrH55nrl9G-200' = 'Manager<br style="">DataConnection<br style="">SessionNavigationService<br style="">NotificationService<br style="">Validator<br style="">EmailMessage<br style="">ImageSourceHelper'
    'ouVfGpyxBDxrH55nrl9G-169' = '<span style="text-align: left;">Commands&nbsp;</span>'
    'ouVfGpyxBDxrH55nrl9G-201' = 'RelayCommand<br style="">FavoriteCommand<br style="">ReservationMasterCommand<br style="">CancelReservationCommand<br style="">DeleteUserCommand<br style="">DeleteSketchCommand<br style="">DeleteReservationCommand'
    'ouVfGpyxBDxrH55nrl9G-170' = 'Data Access (ADO.NET / SQL Server)'
    'ouVfGpyxBDxrH55nrl9G-171' = 'DataConnection'
    'ouVfGpyxBDxrH55nrl9G-172' = '<b>Models<br></b>Acc, User, Admin, Master, Sketch,<br>Reservation, Favourite, Feedback,<br>SupportRequest, InternalNotification'
    'ouVfGpyxBDxrH55nrl9G-173' = 'SQL Server SQLEXPRESS'
    'ouVfGpyxBDxrH55nrl9G-175' = 'Database tables'
    'ouVfGpyxBDxrH55nrl9G-176' = 'Accounts'
    'ouVfGpyxBDxrH55nrl9G-177' = 'Reservations'
    'ouVfGpyxBDxrH55nrl9G-178' = 'Feedbacks'
    'ouVfGpyxBDxrH55nrl9G-179' = 'Masters'
    'ouVfGpyxBDxrH55nrl9G-180' = 'Sketches'
    'ouVfGpyxBDxrH55nrl9G-181' = 'Favourites'
    'ouVfGpyxBDxrH55nrl9G-182' = 'InternalNotifications'
    'ouVfGpyxBDxrH55nrl9G-183' = 'SupportRequests'
    'ouVfGpyxBDxrH55nrl9G-202' = 'TattooShopProject'
    'ouVfGpyxBDxrH55nrl9G-184' = 'File system'
    'ouVfGpyxBDxrH55nrl9G-185' = 'image/*, Resources/*'
    'ouVfGpyxBDxrH55nrl9G-186' = 'Style/Style.xaml, Fonts/*'
    'ouVfGpyxBDxrH55nrl9G-187' = 'App.config, Database/schema.sql'
}

Update-DrawioText -Path $classPath -Values $classValues
Update-DrawioText -Path $architecturePath -Values $architectureValues
