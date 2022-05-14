using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AuthorizeNet.Api.Controllers;
using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers.Bases;
using System.Text.RegularExpressions;
/// <summary>
/// Summary description for AuthorizeAPI
/// </summary>
public class AuthorizeAPI
{
    public AuthorizeAPI()
    {

        apiLoginID = System.Configuration.ConfigurationManager.AppSettings["AuthorizeAPILoginID"];
        apiTransactionKey = System.Configuration.ConfigurationManager.AppSettings["AuthorizeAPITransactionKey"];


    }

    public AuthorizeAPI(string LoginId, string TransactionKey)
    {
        apiLoginID = LoginId;
        apiTransactionKey = TransactionKey;
    }

    private string apiLoginID = "";
    private string apiTransactionKey = "";


    //Set Card Type
    public enum CardType
    {

        //American Express, Discover, JCB, MasterCard, Visa
        Unknown = 0,
        MasterCard = 1,
        VISA = 2,
        Amex = 3,
        Discover = 4,
        DinersClub = 5,
        JCB = 6,
        EnRoute = 7
    }
    // Class to hold credit card type information
    private class CardTypeInfo
    {
        public CardTypeInfo(string regEx, int length, CardType type)
        {
            RegEx = regEx;
            Length = length;
            Type = type;
        }

        public string RegEx { get; set; }
        public int Length { get; set; }
        public CardType Type { get; set; }
    }

    private static CardTypeInfo[] _cardTypeInfo =
    {
      new CardTypeInfo("^(51|52|53|54|55)", 16, CardType.MasterCard),
      new CardTypeInfo("^(4)", 16, CardType.VISA),
      new CardTypeInfo("^(4)", 13, CardType.VISA),
      new CardTypeInfo("^(34|37)", 15, CardType.Amex),
      new CardTypeInfo("^(6011)", 16, CardType.Discover),
      new CardTypeInfo("^(300|301|302|303|304|305|36|38)", 
                       14, CardType.DinersClub),
      new CardTypeInfo("^(3)", 16, CardType.JCB),
      new CardTypeInfo("^(2131|1800)", 15, CardType.JCB),
      new CardTypeInfo("^(2014|2149)", 15, CardType.EnRoute),
    };

    public CardType GetCardType(string cardNumber)
    {
        foreach (CardTypeInfo info in _cardTypeInfo)
        {
            if (cardNumber.Length == info.Length &&
                Regex.IsMatch(cardNumber, info.RegEx))
                return info.Type;
        }

        return CardType.Unknown;
    }

    public List<string> CreateCustomerProfile(string CardNumber, string ExpirationDate, string Name, string Email)
    {

        List<string> sArray = new List<string>();
        try
        {

            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseCreditCard"]))
                ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.PRODUCTION;
            else
                ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;



            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = apiLoginID,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = apiTransactionKey,
            };


            var creditCard = new creditCardType
            {
                cardNumber = CardNumber,
                expirationDate = ExpirationDate

            };


            //standard api call to retrieve response
            paymentType cc = new paymentType { Item = creditCard };
            // paymentType echeck = new paymentType {Item = bankAccount};

            List<customerPaymentProfileType> paymentProfileList = new List<customerPaymentProfileType>();
            customerPaymentProfileType ccPaymentProfile = new customerPaymentProfileType();
            ccPaymentProfile.payment = cc;

            paymentProfileList.Add(ccPaymentProfile);
            // paymentProfileList.Add(echeckPaymentProfile);

            //List<customerAddressType> addressInfoList = new List<customerAddressType>();
            //customerAddressType shipAddress = new customerAddressType();
            //shipAddress.address = "Section 10 Mirpur";
            //shipAddress.city = "Dhaka";
            //shipAddress.zip = "1216";


            // addressInfoList.Add(shipAddress);



            customerProfileType customerProfile = new customerProfileType();
            customerProfile.merchantCustomerId = Name;
            customerProfile.email = Email;
            customerProfile.paymentProfiles = paymentProfileList.ToArray();
            //customerProfile.shipToList = addressInfoList.ToArray();

            var request = new createCustomerProfileRequest { profile = customerProfile, validationMode = validationModeEnum.none };

            var controller = new createCustomerProfileController(request);          // instantiate the contoller that will call the service
            controller.Execute();

            createCustomerProfileResponse response = controller.GetApiResponse();   // get the response from the service (errors contained if any)

            //validate
            if (response != null && response.messages.resultCode == messageTypeEnum.Ok)
            {
                if (response != null && response.messages.message != null)
                {
                    sArray.Add(response.customerProfileId);
                    sArray.Add(response.customerPaymentProfileIdList[0]);
                }
            }
            else
            {
                throw new Exception(response.messages.message[0].code + "  " + response.messages.message[0].text);

            }

        }
        catch (Exception ex)
        {
            throw ex;
        }

        return sArray;
    }

    public List<string> CreateCustomerProfile(string CardNumber, string ExpirationDate, string ProfileName, string FirstName, string Email, string Address, string City, string State, string Zip)
    {

        List<string> sArray = new List<string>();
        try
        {

            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseCreditCard"]))
                ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.PRODUCTION;
            else
                ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;



            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = apiLoginID,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = apiTransactionKey,
            };


            var creditCard = new creditCardType
            {
                cardNumber = CardNumber,
                expirationDate = ExpirationDate


            };


            customerAddressType billTo = new customerAddressType();
            billTo.firstName = FirstName;
            billTo.email = Email;
            billTo.address = Address;
            billTo.city = City;
            billTo.state = State;
            billTo.zip = Zip;




            //standard api call to retrieve response
            paymentType cc = new paymentType { Item = creditCard };
            // paymentType echeck = new paymentType {Item = bankAccount};

            List<customerPaymentProfileType> paymentProfileList = new List<customerPaymentProfileType>();
            customerPaymentProfileType ccPaymentProfile = new customerPaymentProfileType();
            ccPaymentProfile.payment = cc;
            ccPaymentProfile.billTo = billTo;
            paymentProfileList.Add(ccPaymentProfile);
            // paymentProfileList.Add(echeckPaymentProfile);



            customerProfileType customerProfile = new customerProfileType();
            customerProfile.merchantCustomerId = ProfileName;
            customerProfile.email = Email;
            customerProfile.paymentProfiles = paymentProfileList.ToArray();
            //customerProfile.billshipToList = addressInfoList.ToArray();

            var request = new createCustomerProfileRequest { profile = customerProfile, validationMode = validationModeEnum.none };

            var controller = new createCustomerProfileController(request);          // instantiate the contoller that will call the service
            controller.Execute();

            createCustomerProfileResponse response = controller.GetApiResponse();   // get the response from the service (errors contained if any)

            //validate
            if (response != null && response.messages.resultCode == messageTypeEnum.Ok)
            {
                if (response != null && response.messages.message != null)
                {
                    sArray.Add(response.customerProfileId);
                    sArray.Add(response.customerPaymentProfileIdList[0]);
                }
            }
            else
            {
                throw new Exception(response.messages.message[0].code + "  " + response.messages.message[0].text);

            }

        }
        catch (Exception ex)
        {
            throw ex;
        }

        return sArray;
    }


    public List<string> CreateCustomerProfileFromTransaction(string TransactionId)
    {

        List<string> sArray = new List<string>();
        try
        {

            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseCreditCard"]))
                ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.PRODUCTION;
            else
                ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;

            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
           {
               name = apiLoginID,
               ItemElementName = ItemChoiceType.transactionKey,
               Item = apiTransactionKey,
           };

            var request = new createCustomerProfileFromTransactionRequest { transId = TransactionId };

            var controller = new createCustomerProfileFromTransactionController(request);
            controller.Execute();

            createCustomerProfileResponse response = controller.GetApiResponse();


            //validate
            if (response != null && response.messages.resultCode == messageTypeEnum.Ok)
            {
                if (response != null && response.messages.message != null)
                {
                    sArray.Add(response.customerProfileId);
                    sArray.Add(response.customerPaymentProfileIdList[0]);
                }
            }
            else
            {
                throw new Exception(response.messages.message[0].code + "  " + response.messages.message[0].text);

            }

        }
        catch (Exception ex)
        {
            throw ex;
        }

        return sArray;
    }


    public string CreatePaymentProfile(string CardNumber, string ExpirationDate, string CustomerProfileId)
    {

        string sPaymentId = "";
        try
        {
            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseCreditCard"]))
                ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.PRODUCTION;
            else
                ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;



            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = apiLoginID,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = apiTransactionKey,
            };


            var creditCard = new creditCardType
              {
                  cardNumber = CardNumber,
                  expirationDate = ExpirationDate

              };


            //standard api call to retrieve response
            paymentType cc = new paymentType { Item = creditCard };

            customerPaymentProfileType ccPaymentProfile = new customerPaymentProfileType();
            ccPaymentProfile.payment = cc;

            var request = new createCustomerPaymentProfileRequest
            {
                customerProfileId = CustomerProfileId,
                paymentProfile = ccPaymentProfile,
                validationMode = validationModeEnum.none
            };

            //Prepare Request
            var controller = new createCustomerPaymentProfileController(request);
            controller.Execute();

            //Send Request to EndPoint
            createCustomerPaymentProfileResponse response = controller.GetApiResponse();
            if (response != null && response.messages.resultCode == messageTypeEnum.Ok)
            {
                if (response != null && response.messages.message != null)
                {
                    sPaymentId = response.customerPaymentProfileId;
                }
            }
            else
            {
                throw new Exception(response.messages.message[0].text);

            }



        }
        catch (Exception ex)
        {
            throw ex;
        }


        return sPaymentId;
    }

    public string CreatePaymentProfile(string CardNumber, string ExpirationDate, string CustomerProfileId, string FirstName, string Email, string Address, string City, string State, string Zip)
    {

        string sPaymentId = "";
        try
        {
            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseCreditCard"]))
                ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.PRODUCTION;
            else
                ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;



            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = apiLoginID,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = apiTransactionKey,
            };


            var creditCard = new creditCardType
              {
                  cardNumber = CardNumber,
                  expirationDate = ExpirationDate

              };


            customerAddressType billTo = new customerAddressType();
            billTo.firstName = FirstName;
            billTo.email = Email;
            billTo.address = Address;
            billTo.city = City;
            billTo.state = State;
            billTo.zip = Zip;

            //standard api call to retrieve response
            paymentType cc = new paymentType { Item = creditCard };

            customerPaymentProfileType ccPaymentProfile = new customerPaymentProfileType();
            ccPaymentProfile.payment = cc;
            ccPaymentProfile.billTo = billTo;

            var request = new createCustomerPaymentProfileRequest
            {
                customerProfileId = CustomerProfileId,
                paymentProfile = ccPaymentProfile,
                validationMode = validationModeEnum.none
            };

            //Prepare Request
            var controller = new createCustomerPaymentProfileController(request);
            controller.Execute();

            //Send Request to EndPoint
            createCustomerPaymentProfileResponse response = controller.GetApiResponse();
            if (response != null && response.messages.resultCode == messageTypeEnum.Ok)
            {
                if (response != null && response.messages.message != null)
                {
                    sPaymentId = response.customerPaymentProfileId;
                }
            }
            else
            {
                throw new Exception(response.messages.message[0].text);

            }



        }
        catch (Exception ex)
        {
            throw ex;
        }


        return sPaymentId;
    }


    public bool UpdatePaymentProfile(string CardNumber, string ExpirationDate, string CustomerProfileId, string PaymentProfileId)
    {



        try
        {
            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseCreditCard"]))
                ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.PRODUCTION;
            else
                ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;



            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = apiLoginID,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = apiTransactionKey,
            };



            var creditCard = new creditCardType
                  {

                      cardNumber = CardNumber,
                      expirationDate = ExpirationDate
                  };

            var paymentType = new paymentType { Item = creditCard };

            var paymentProfile = new customerPaymentProfileExType
            {
                payment = paymentType,
                customerPaymentProfileId = PaymentProfileId
            };

            var request = new updateCustomerPaymentProfileRequest();
            request.customerProfileId = CustomerProfileId;
            request.paymentProfile = paymentProfile;
            request.validationMode = validationModeEnum.none;


            // instantiate the controller that will call the service
            var controller = new updateCustomerPaymentProfileController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();

            if (response.messages.resultCode == messageTypeEnum.Ok)
            {
                return true;
            }
            else
            {
                throw new Exception(response.messages.message[0].text);


            }



        }
        catch (Exception ex)
        {
            throw ex;
        }


    }

    public bool UpdatePaymentProfile(string CardNumber, string ExpirationDate, string CustomerProfileId, string PaymentProfileId, string FirstName, string Email, string Address, string City, string State, string Zip)
    {



        try
        {
            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseCreditCard"]))
                ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.PRODUCTION;
            else
                ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;



            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = apiLoginID,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = apiTransactionKey,
            };


            customerAddressType billTo = new customerAddressType();
            billTo.firstName = FirstName;

            billTo.email = Email;
            billTo.address = Address;
            billTo.city = City;
            billTo.state = State;
            billTo.zip = Zip;

            var creditCard = new creditCardType
                  {

                      cardNumber = CardNumber,
                      expirationDate = ExpirationDate
                  };

            var paymentType = new paymentType { Item = creditCard };

            var paymentProfile = new customerPaymentProfileExType
            {
                payment = paymentType,
                customerPaymentProfileId = PaymentProfileId,
                billTo = billTo
            };

            var request = new updateCustomerPaymentProfileRequest();
            request.customerProfileId = CustomerProfileId;
            request.paymentProfile = paymentProfile;
            request.validationMode = validationModeEnum.none;


            // instantiate the controller that will call the service
            var controller = new updateCustomerPaymentProfileController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();

            if (response.messages.resultCode == messageTypeEnum.Ok)
            {
                return true;
            }
            else
            {
                throw new Exception(response.messages.message[0].text);


            }



        }
        catch (Exception ex)
        {
            throw ex;
        }


    }


    public bool DeletePaymentProfile(string CustomerProfileId, string PaymentProfileId)
    {



        try
        {
            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseCreditCard"]))
                ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.PRODUCTION;
            else
                ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;



            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = apiLoginID,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = apiTransactionKey,
            };


            //please update the subscriptionId according to your sandbox credentials
            var request = new deleteCustomerPaymentProfileRequest
            {
                customerProfileId = CustomerProfileId,
                customerPaymentProfileId = PaymentProfileId
            };

            //Prepare Request
            var controller = new deleteCustomerPaymentProfileController(request);
            controller.Execute();

            //Send Request to EndPoint
            deleteCustomerPaymentProfileResponse response = controller.GetApiResponse();

            if (response.messages.resultCode == messageTypeEnum.Ok)
            {
                return true;
            }
            else
            {
                throw new Exception(response.messages.message[0].text);
            }



        }
        catch (Exception ex)
        {
            throw ex;
        }


    }


    public string ChargeCustomerProfile(string CustomerProfileId, string PaymentProfileId, decimal Amount)
    {

        string sReturn = "";
        try
        {
            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseCreditCard"]))
                ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.PRODUCTION;
            else
                ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;

            // define the merchant information (authentication / transaction id)
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = apiLoginID,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = apiTransactionKey
            };

            //create a customer payment profile
            customerProfilePaymentType profileToCharge = new customerProfilePaymentType();
            profileToCharge.customerProfileId = CustomerProfileId;
            profileToCharge.paymentProfile = new paymentProfile { paymentProfileId = PaymentProfileId };

            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.authCaptureTransaction.ToString(),    // refund type
                amount = Amount,
                profile = profileToCharge
            };

            var request = new createTransactionRequest { transactionRequest = transactionRequest };

            // instantiate the collector that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();

            //validate
            if (response.messages.resultCode == messageTypeEnum.Ok)
            {
                if (response.transactionResponse != null)
                {

                    if (response.transactionResponse.messages != null)
                    {
                        if (response.transactionResponse.messages[0].code.ToString() == "1" && response.transactionResponse.authCode.Length > 0)
                        {
                            sReturn = response.transactionResponse.transId;
                        }
                        else
                        {
                            throw new Exception(response.transactionResponse.messages[0].description);

                        }
                    }
                    else if (response.transactionResponse.errors != null)
                    { 
                     throw new Exception(response.transactionResponse.errors[0].errorText);
                    
                    }
                }
            }
            else
            {
                if (response.transactionResponse.errors != null)
                    throw new Exception(response.transactionResponse.errors[0].errorText);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return sReturn;
    }


    public string ChargeCreditCard(string CardNumber, string ExpirationDate, string CardCode, decimal Amount, string Name, string Email, string Address, string City, string State, string Zip)
    {
        string sReturn = "";

        try
        {
            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseCreditCard"]))
                ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.PRODUCTION;
            else
                ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;


            // define the merchant information (authentication / transaction id)
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = apiLoginID,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = apiTransactionKey,
            };


            customerAddressType billTo = new customerAddressType();
            billTo.firstName = Name;

            billTo.email = Email;
            billTo.address = Address;
            billTo.city = City;
            billTo.state = State;
            billTo.zip = Zip;


            var creditCard = new creditCardType
            {
                cardNumber = CardNumber,
                expirationDate = ExpirationDate,
                cardCode = CardCode

            };

            //standard api call to retrieve response
            var paymentType = new paymentType { Item = creditCard };

            // Add line Items
            //var lineItems = new lineItemType[2];
            //lineItems[0] = new lineItemType { itemId = "1", name = "t-shirt", quantity = 2, unitPrice = new Decimal(15.00) };
            //lineItems[1] = new lineItemType { itemId = "2", name = "snowboard", quantity = 1, unitPrice = new Decimal(450.00) };


            customerDataType customerType = new customerDataType();
            customerType.id = Name;
            customerType.email = Email;


            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.authCaptureTransaction.ToString(),    // charge the card
                amount = Amount,
                payment = paymentType,
                customer = customerType,
                billTo = billTo


                //,lineItems = lineItems
            };



            var request = new createTransactionRequest { transactionRequest = transactionRequest };

            // instantiate the contoller that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();

            if (response!=null && response.messages.resultCode == messageTypeEnum.Ok)
            {
                if (response.transactionResponse != null)
                {

                    if (response.transactionResponse.messages != null)
                    {
                        if (response.transactionResponse.messages[0].code.ToString() == "1" && response.transactionResponse.authCode.Length > 0)
                        {
                            sReturn = response.transactionResponse.transId;
                        }
                        else
                        {
                            throw new Exception(response.transactionResponse.messages[0].description);

                        }
                    }
                    else if (response.transactionResponse.errors != null)
                    { 
                     throw new Exception(response.transactionResponse.errors[0].errorText);
                    
                    }
                }
            }
            else
            {
                if (response.transactionResponse.errors != null)
                    throw new Exception(response.transactionResponse.errors[0].errorText);
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
        return sReturn;

    }


    //public string ChargeCreditCard(string CardNumber, string ExpirationDate, string CardCode, decimal Amount, string Name, string Email)
    //{
    //    string sReturn = "";

    //    try
    //    {
    //        if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseCreditCard"]))
    //            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.PRODUCTION;
    //        else
    //            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;


    //        // define the merchant information (authentication / transaction id)
    //        ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
    //        {
    //            name = apiLoginID,
    //            ItemElementName = ItemChoiceType.transactionKey,
    //            Item = apiTransactionKey,
    //        };

    //        var creditCard = new creditCardType
    //        {
    //            cardNumber = CardNumber,
    //            expirationDate = ExpirationDate,
    //            cardCode = CardCode
    //        };

    //        //standard api call to retrieve response
    //        var paymentType = new paymentType { Item = creditCard };

    //        // Add line Items
    //        //var lineItems = new lineItemType[2];
    //        //lineItems[0] = new lineItemType { itemId = "1", name = "t-shirt", quantity = 2, unitPrice = new Decimal(15.00) };
    //        //lineItems[1] = new lineItemType { itemId = "2", name = "snowboard", quantity = 1, unitPrice = new Decimal(450.00) };


    //        customerDataType customerType = new customerDataType();
    //        customerType.id = Name;
    //        customerType.email = Email;

    //        var transactionRequest = new transactionRequestType
    //        {
    //            transactionType = transactionTypeEnum.authCaptureTransaction.ToString(),    // charge the card
    //            amount = Amount,
    //            payment = paymentType,
    //            customer = customerType


    //            //,lineItems = lineItems
    //        };



    //        var request = new createTransactionRequest { transactionRequest = transactionRequest };

    //        // instantiate the contoller that will call the service
    //        var controller = new createTransactionController(request);
    //        controller.Execute();

    //        // get the response from the service (errors contained if any)
    //        var response = controller.GetApiResponse();

    //         if (response.messages.resultCode == messageTypeEnum.Ok)
    //        {
    //            if (response.transactionResponse != null)
    //            {

    //                if (response.transactionResponse.messages != null)
    //                {
    //                    if (response.transactionResponse.messages[0].code.ToString() == "1" && response.transactionResponse.authCode.Length > 0)
    //                    {
    //                        sReturn = response.transactionResponse.transId;
    //                    }
    //                    else
    //                    {
    //                        throw new Exception(response.transactionResponse.messages[0].description);

    //                    }
    //                }
    //                else if (response.transactionResponse.errors != null)
    //                { 
    //                 throw new Exception(response.transactionResponse.errors[0].errorText);
                    
    //                }
    //            }
    //        }
    //        else
    //        {
    //            if (response.transactionResponse.errors != null)
    //                throw new Exception(response.transactionResponse.errors[0].errorText);
    //        }

    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    return sReturn;

    //}

    public string ChargeECheck(string AccountNumber, string RoutingNumber, string NameOnAccount, string BankName, decimal Amount)
    {
        string sReturn = "";

        try
        {
            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseCreditCard"]))
                ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.PRODUCTION;
            else
                ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;


            // define the merchant information (authentication / transaction id)
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = apiLoginID,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = apiTransactionKey,
            };


            var bankAccount = new bankAccountType
            {
                accountNumber = AccountNumber,
                routingNumber = RoutingNumber,
                accountType = bankAccountTypeEnum.checking,
                echeckType = echeckTypeEnum.WEB,
                nameOnAccount = NameOnAccount,
                bankName = BankName
            };

            paymentType echeck = new paymentType { Item = bankAccount };

            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.authCaptureTransaction.ToString(),    // charge the card
                amount = Amount,
                payment = echeck
            };




            var request = new createTransactionRequest { transactionRequest = transactionRequest };

            // instantiate the contoller that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();
            if (response == null)
            {
                throw new Exception("Bank or Account information is invalid");
            }

            if (response.messages.resultCode == messageTypeEnum.Ok)
            {
                if (response.transactionResponse != null)
                {

                    if (response.transactionResponse.messages != null)
                    {
                        if (response.transactionResponse.messages[0].code.ToString() == "1")
                        {
                            sReturn = response.transactionResponse.transId;
                        }
                        else
                        {
                            throw new Exception(response.transactionResponse.messages[0].description);

                        }
                    }
                    else if (response.transactionResponse.errors != null)
                    {
                        throw new Exception(response.transactionResponse.errors[0].errorText);

                    }
                }
            }
            else
            {
                if (response.transactionResponse.errors != null)
                    throw new Exception(response.transactionResponse.errors[0].errorText);
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
        return sReturn;

    }

}

