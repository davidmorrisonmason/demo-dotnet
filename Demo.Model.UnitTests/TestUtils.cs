using Demo.Model.Domain;
using Demo.Model.Domain.Validation;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

namespace Demo.Model.UnitTests;

/// <summary>
/// Encapsulation of all test assertion utilities in one place to make it easy to move if desired
/// </summary>
public static class TestUtils
{
    #region Equal

    public static void ShouldEqual<T>(this T? actual, T? expected)
    {
        actual.Should().Be(expected);
    }

    #endregion

    #region Boolean

    public static void ShouldBeTrue(this bool actual)
    {
        actual.Should().BeTrue();
    }
    public static void ShouldBeFalse(this bool actual)
    {
        actual.Should().BeFalse();
    }

    #endregion

    #region Equivalent

    public static void ShouldBeEquivalentTo<T>(this T? actual, T? expected) where T : class
    {
        actual.Should().BeEquivalentTo(expected);
    }

    public static void ShouldBeEquivalentTo<T>(this T actual, T expected, Func<EquivalencyOptions<T>, EquivalencyOptions<T>> options = null) where T : class
    {
        // if no explicit options passed in, use a fuzzy datetime match by default
        if (options == null)
        {
            actual.Should().BeEquivalentTo(expected, opts =>
            opts
                .Using<DateTime>(ctx => ctx.Subject.ShouldBeCloseTo(ctx.Expectation))
                    .WhenTypeIs<DateTime>()
                .Using<Dictionary<string, object>>(ctx => ctx.Subject.ShouldBeEquivalentTo(ctx.Expectation))
                    .WhenTypeIs<Dictionary<string, object>>());

        }
        else
        {
            Func<EquivalencyOptions<T>, EquivalencyOptions<T>> effectiveOptions = opts =>
            {
                opts
                    .Using<string>(ctx =>
                    {
                        if (DateTime.TryParse(ctx.Subject, out DateTime actualDateTime) && DateTime.TryParse(ctx.Expectation.ToString(), out DateTime expectedDateTime))
                        {
                            actualDateTime.ShouldBeCloseTo(expectedDateTime);
                        }
                        else
                        {
                            ctx.Subject.ShouldEqual(ctx.Expectation);
                        }
                    })
                        .WhenTypeIs<string>()
                    .Using<DateTime>(ctx => ctx.Subject.ShouldBeCloseTo(ctx.Expectation))
                        .WhenTypeIs<DateTime>()
                    .Using<Dictionary<string, object>>(ctx => ctx.Subject.ShouldBeEquivalentTo(ctx.Expectation))
                        .WhenTypeIs<Dictionary<string, object>>();

                return options(opts);
            };

            actual.Should().BeEquivalentTo(expected, effectiveOptions);
        }
    }

    #endregion

    #region Null

    public static void ShouldBeNull<T>(this T? actual) where T : class
    {
        actual.Should().BeNull();
    }

    public static void ShouldNotBeNull<T>(this T actual) where T : class
    {
        actual.Should().NotBeNull();
    }

    #endregion

    #region Date/Time

    public static void ShouldBeCloseTo(this DateTime actualDateTime, DateTime expectedDateTime, int secondsTolerance = 10)
    {
        actualDateTime.Should().BeCloseTo(expectedDateTime, TimeSpan.FromSeconds(secondsTolerance));
    }

    #endregion

    #region API Response

    public static TDto ShouldBeOkResponse<TDto>(this HttpResponseMessage response, TDto expected,
        Func<EquivalencyOptions<TDto>, EquivalencyOptions<TDto>> options = null) where TDto : class
    {
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = ParseHttpResponseAsJObject(response);
        var actual = JsonConvert.DeserializeObject<TDto>(body.ToString());
        actual.ShouldBeEquivalentTo(expected, options);
        return actual;
    }

    public static IEnumerable<TDto> ShouldBeOkListResponse<TDto>(
        this HttpResponseMessage response,
        IEnumerable<TDto> expected,
        Func<EquivalencyOptions<TDto>, EquivalencyOptions<TDto>> options = null) where TDto : class
    {
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = ParseHttpResponseAsJArray(response);
        var actual = body.Select(x => JsonConvert.DeserializeObject<TDto>(x.ToString())).ToList();
        var expectedDtos = expected.ToList();
        actual.Count().ShouldEqual(expectedDtos.Count());
        for (int i = 0; i < actual.Count(); i++)
        {
            actual[i].ShouldBeEquivalentTo(expectedDtos[i], options);
        }
        return actual;
    }

    private static JObject ParseHttpResponseAsJObject(HttpResponseMessage response)
    {
        var content = GetResponseContent(response);
        var json = JObject.Parse(content);
        return json;
    }

    private static JArray ParseHttpResponseAsJArray(HttpResponseMessage response)
    {
        var content = GetResponseContent(response);
        var json = JArray.Parse(content);
        return json;
    }

    private static string GetResponseContent(HttpResponseMessage response)
    {
        var stream = response.Content.ReadAsStreamAsync().Result;
        var reader = new StreamReader(stream);
        var content = reader.ReadToEnd();
        return content;
    }

    public static void ShouldBeModelValidationErrorResponse(this HttpResponseMessage response, ErrorMessage expectedErrorMessage)
    {
        response.ShouldBeModelValidationErrorResponse(new List<ErrorMessage> { expectedErrorMessage });
    }

    public static void ShouldBeModelValidationErrorResponse(this HttpResponseMessage response, IEnumerable<ErrorMessage> expectedErrorMessages)
    {
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var errors = ParseHttpResponseAsJObject(response)["errors"] as JArray;
        errors.ShouldNotBeNull();
        var actual = errors.Select(x => new ErrorMessage(x["errorCode"]?.ToString(), x["errorDescription"]?.ToString()));
        actual.ShouldBeEquivalentTo(expectedErrorMessages);
    }

    public static void ShouldBeNotFoundErrorResponse(this HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public static int ShouldBeCreatedResponse(this HttpResponseMessage response)
    {
        var body = ParseHttpResponseAsJObject(response);
        response.StatusCode.Should().Be(HttpStatusCode.Created, because: $"Response should be 201 Created and should not contain errors: {body}");
        body.ContainsKey("id").ShouldBeTrue();
        int.TryParse(body["id"].ToString(), out int createdId).ShouldBeTrue();

        return createdId;
    }

    public static void ShouldBeNoContentResponse(this HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    #endregion

    #region Database

    public static void ShouldBeInDatabase<T>(this T expected) where T : DomainObject
    {
        using var dbContext = TestContext.DbContext;
        var actual = dbContext.Set<T>().FirstOrDefault(x => x.Id == expected.Id);
        actual.ShouldBeEquivalentTo(expected);
    }
    public static void ShouldAllBeInDatabase<T>(this IEnumerable<T> expected) where T : DomainObject
    {
        using var dbContext = TestContext.DbContext;
        var expectedIds = expected.Select(x => x.Id);
        var actual = dbContext.Set<T>().Where(x => expectedIds.Contains(x.Id)).ToList();
        actual.ShouldBeEquivalentTo(expected);
    }

    #endregion
}
