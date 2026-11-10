using DonationHub.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Entities.Categories
{
    public static class CategoryErrors
    {
        public static Error NameArRequired => Error.Validation(
            "Category.NameArRequired",
            "Arabic name is required.");

        public static Error NameEnRequired => Error.Validation("Category.NameEnRequired", "English name is required.");
        public static readonly Error NameArTooLong =
       Error.Validation(
           "Category.NameArTooLong",
           "Arabic name cannot exceed 100 characters.");

        public static readonly Error NameEnTooLong =
            Error.Validation(
                "Category.NameEnTooLong",
                "English name cannot exceed 100 characters.");


        // Conflict Errors
        public static readonly Error NameArAlreadyExists =
            Error.Conflict(
                "Category.NameArAlreadyExists",
                "Arabic category name already exists.");

        public static readonly Error NameEnAlreadyExists =
            Error.Conflict(
                "Category.NameEnAlreadyExists",
                "English category name already exists.");

        public static readonly Error HasCampaigns =
            Error.Conflict(
                "Category.HasCampaigns",
                "Category cannot be deleted because it has campaigns.");


        // Not Found Errors
        public static readonly Error NotFound =
            Error.NotFound(
                "Category.NotFound",
                "Category was not found.");

    }
}
