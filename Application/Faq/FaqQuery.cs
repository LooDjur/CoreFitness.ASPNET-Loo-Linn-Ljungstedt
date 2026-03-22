using Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Faq;

public record FaqQuery() : IRequest<Result<IReadOnlyList<FaqItem>>>;