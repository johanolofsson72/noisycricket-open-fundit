using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace AppClient.Data;


    public class VatNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("VAT number is required.");
            }

            var vatNumber = value.ToString();
            // Add your VAT number validation logic here. This is just an example regex for Swedish VAT number.
            var vatRegex = new Regex(
                @"^(ATU[0-9]{8})|" +      // Austria
                @"(BE0[0-9]{9})|" +      // Belgium
                @"(BG[0-9]{9,10})|" +    // Bulgaria
                @"(CY[0-9]{8}L)|" +      // Cyprus
                @"(CZ[0-9]{8,10})|" +    // Czech Republic
                @"(DE[0-9]{9})|" +       // Germany
                @"(DK[0-9]{8})|" +       // Denmark
                @"(EE[0-9]{9})|" +       // Estonia
                @"(EL[0-9]{9})|" +       // Greece
                @"(ES[A-Z0-9]{1}[0-9]{7}[A-Z0-9]{1})|" + // Spain
                @"(FI[0-9]{8})|" +       // Finland
                @"(FR[A-HJ-NP-Z0-9]{2}[0-9]{9})|" + // France
                @"(GB([0-9]{9}|[0-9]{12}|\d{3}\s\d{4}\s\d{2}\s\d{3}))|" + // United Kingdom
                @"(HR[0-9]{11})|" +      // Croatia
                @"(HU[0-9]{8})|" +       // Hungary
                @"(IE[0-9]{7}[A-W])|" +  // Ireland
                @"(IT[0-9]{11})|" +      // Italy
                @"(LT([0-9]{9}|[0-9]{12}))|" + // Lithuania
                @"(LU[0-9]{8})|" +       // Luxembourg
                @"(LV[0-9]{11})|" +      // Latvia
                @"(MT[0-9]{8})|" +       // Malta
                @"(NL[0-9]{9}B[0-9]{2})|" + // Netherlands
                @"(PL[0-9]{10})|" +      // Poland
                @"(PT[0-9]{9})|" +       // Portugal
                @"(RO[0-9]{2,10})|" +    // Romania
                @"(SE[0-9]{12})|" +      // Sweden
                @"(SI[0-9]{8})|" +       // Slovenia
                @"(SK[0-9]{10})$"        // Slovakia
            );

            return (vatNumber != null && !vatRegex.IsMatch(vatNumber) ? new ValidationResult("Invalid VAT number format.") : ValidationResult.Success) ?? new ValidationResult("VAT number is mal formed.");;
        }
    }
    