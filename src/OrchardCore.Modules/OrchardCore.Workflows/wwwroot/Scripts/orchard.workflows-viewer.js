/*
** NOTE: This file is generated by Gulp and should not be edited directly!
** Any changes made directly to this file will be overwritten next time its asset group is processed by Gulp.
*/

///<reference path='../Lib/jquery/typings.d.ts' />
///<reference path='../Lib/jsplumb/typings.d.ts' />
///<reference path='./workflow-models.ts' />
var WorkflowCanvas = /** @class */ (function () {
    function WorkflowCanvas(container, workflowType) {
        var _this = this;
        this.container = container;
        this.workflowType = workflowType;
        this.minCanvasHeight = 400;
        this.getActivityElements = function () {
            return $(_this.container).find('.activity');
        };
        this.getDefaults = function () {
            return {
                Anchor: "Continuous",
                DragOptions: { cursor: 'pointer', zIndex: 2000 },
                EndpointStyles: [{ fillStyle: '#225588' }],
                Endpoints: [["Dot", { radius: 7 }], ["Blank"]],
                ConnectionOverlays: [
                    ["Arrow", { width: 12, length: 12, location: -5 }],
                ],
                ConnectorZIndex: 5
            };
        };
        this.createJsPlumbInstance = function () {
            return jsPlumb.getInstance({
                DragOptions: { cursor: 'pointer', zIndex: 2000 },
                ConnectionOverlays: [
                    ['Arrow', {
                            location: 1,
                            visible: true,
                            width: 11,
                            length: 11
                        }],
                    ['Label', {
                            location: 0.5,
                            id: 'label',
                            cssClass: 'connection-label'
                        }]
                ],
                Container: _this.container
            });
        };
        this.getEndpointColor = function (activity) {
            return activity.isBlocking || activity.isStart ? '#7ab02c' : activity.isEvent ? '#3a8acd' : '#7ab02c';
        };
        this.getSourceEndpointOptions = function (activity, outcome) {
            // The definition of source endpoints.
            var paintColor = _this.getEndpointColor(activity);
            return {
                endpoint: 'Dot',
                anchor: 'Continuous',
                paintStyle: {
                    stroke: paintColor,
                    fill: paintColor,
                    radius: 7,
                    strokeWidth: 1
                },
                isSource: true,
                connector: ['Flowchart', { stub: [40, 60], gap: 0, cornerRadius: 5, alwaysRespectStubs: true }],
                connectorStyle: {
                    strokeWidth: 2,
                    stroke: '#999999',
                    joinstyle: 'round',
                    outlineStroke: 'white',
                    outlineWidth: 2
                },
                hoverPaintStyle: {
                    fill: '#216477',
                    stroke: '#216477'
                },
                connectorHoverStyle: {
                    strokeWidth: 3,
                    stroke: '#216477',
                    outlineWidth: 5,
                    outlineStroke: 'white'
                },
                connectorOverlays: [['Label', { location: [3, -1.5], cssClass: 'endpointSourceLabel' }]],
                dragOptions: {},
                uuid: activity.id + "-" + outcome.name,
                parameters: {
                    outcome: outcome
                }
            };
        };
        this.getActivity = function (id, activities) {
            if (activities === void 0) { activities = null; }
            if (!activities) {
                activities = this.workflowType.activities;
            }
            return $.grep(activities, function (x) { return x.id === id; })[0];
        };
        this.updateConnections = function (plumber) {
            var workflowId = _this.workflowType.id;
            // Connect activities.
            for (var _i = 0, _a = _this.workflowType.transitions; _i < _a.length; _i++) {
                var transitionModel = _a[_i];
                var sourceEndpointUuid = transitionModel.sourceActivityId + "-" + transitionModel.sourceOutcomeName;
                var sourceEndpoint = plumber.getEndpoint(sourceEndpointUuid);
                var destinationElementId = "activity-" + workflowId + "-" + transitionModel.destinationActivityId;
                plumber.connect({
                    source: sourceEndpoint,
                    target: destinationElementId
                });
            }
        };
        this.updateCanvasHeight = function () {
            var $container = $(this.container);
            // Get the activity element with the highest Y coordinate.
            var $activityElements = $container.find(".activity");
            var currentElementTop = 0;
            var currentActivityHeight = 0;
            for (var _i = 0, _a = $activityElements.toArray(); _i < _a.length; _i++) {
                var activityElement = _a[_i];
                var $activityElement = $(activityElement);
                var top_1 = $activityElement.position().top;
                if (top_1 > currentElementTop) {
                    currentElementTop = top_1;
                    currentActivityHeight = $activityElement.height();
                }
            }
            var newCanvasHeight = currentElementTop + currentActivityHeight;
            var elementBottom = currentElementTop + currentActivityHeight;
            var stretchValue = 100;
            if (newCanvasHeight - elementBottom <= stretchValue) {
                newCanvasHeight += stretchValue;
            }
            $container.height(Math.max(this.minCanvasHeight, newCanvasHeight));
        };
    }
    return WorkflowCanvas;
}());

///<reference path='../Lib/jquery/typings.d.ts' />
///<reference path='../Lib/jsplumb/typings.d.ts' />
///<reference path='./workflow-models.ts' />
///<reference path='./workflow-canvas.ts' />
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var WorkflowViewer = /** @class */ (function (_super) {
    __extends(WorkflowViewer, _super);
    function WorkflowViewer(container, workflowType) {
        var _this = _super.call(this, container, workflowType) || this;
        _this.container = container;
        _this.workflowType = workflowType;
        _this.getEndpointColor = function (activity) {
            return activity.isBlocking ? '#7ab02c' : activity.isEvent ? '#3a8acd' : '#7ab02c';
        };
        var self = _this;
        jsPlumb.ready(function () {
            jsPlumb.importDefaults(_this.getDefaults());
            var plumber = _this.createJsPlumbInstance();
            // Listen for new connections.
            plumber.bind('connection', function (connInfo, originalEvent) {
                var connection = connInfo.connection;
                var outcome = connection.getParameters().outcome;
                var label = connection.getOverlay('label');
                label.setLabel(outcome.displayName);
            });
            var activityElements = _this.getActivityElements();
            var areEqualOutcomes = function (outcomes1, outcomes2) {
                if (outcomes1.length != outcomes2.length) {
                    return false;
                }
                for (var i = 0; i < outcomes1.length; i++) {
                    var outcome1 = outcomes1[i];
                    var outcome2 = outcomes2[i];
                    if (outcome1.name != outcome2.displayName || outcome1.displayName != outcome2.displayName) {
                        return false;
                    }
                }
                return true;
            };
            // Suspend drawing and initialize.
            plumber.batch(function () {
                var workflowId = _this.workflowType.id;
                activityElements.each(function (index, activityElement) {
                    var $activityElement = $(activityElement);
                    var activityId = $activityElement.data('activity-id');
                    var activity = _this.getActivity(activityId);
                    // Configure the activity as a target.
                    plumber.makeTarget(activityElement, {
                        dropOptions: { hoverClass: 'hover' },
                        anchor: 'Continuous',
                        endpoint: ['Blank', { radius: 8 }]
                    });
                    // Add source endpoints.
                    for (var _i = 0, _a = activity.outcomes; _i < _a.length; _i++) {
                        var outcome = _a[_i];
                        var sourceEndpointOptions = _this.getSourceEndpointOptions(activity, outcome);
                        plumber.addEndpoint(activityElement, { connectorOverlays: [['Label', { label: outcome.displayName, cssClass: 'connection-label' }]] }, sourceEndpointOptions);
                    }
                });
                // Connect activities.
                _this.updateConnections(plumber);
                // Re-query the activity elements.
                activityElements = _this.getActivityElements();
                // Make all activity elements visible.
                activityElements.show();
                _this.updateCanvasHeight();
            });
            _this.jsPlumbInstance = plumber;
        });
        return _this;
    }
    return WorkflowViewer;
}(WorkflowCanvas));
$.fn.workflowViewer = function () {
    this.each(function (index, element) {
        var $element = $(element);
        var workflowType = $element.data('workflow-type');
        $element.data('workflowViewer', new WorkflowViewer(element, workflowType));
    });
    return this;
};
$(document).ready(function () {
    var workflowViewer = $('.workflow-canvas').workflowViewer().data('workflowViewer');
});

//# sourceMappingURL=data:application/json;charset=utf8;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIndvcmtmbG93LWNhbnZhcy50cyIsIndvcmtmbG93LXZpZXdlci50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiOzs7OztBQUFBLGtEQUFrRDtBQUNsRCxtREFBbUQ7QUFDbkQsNENBQTRDO0FBRTVDO0lBR0ksd0JBQXNCLFNBQXNCLEVBQVksWUFBb0M7UUFBNUYsaUJBQ0M7UUFEcUIsY0FBUyxHQUFULFNBQVMsQ0FBYTtRQUFZLGlCQUFZLEdBQVosWUFBWSxDQUF3QjtRQUZwRixvQkFBZSxHQUFXLEdBQUcsQ0FBQztRQUs1Qix3QkFBbUIsR0FBRztZQUM1QixPQUFPLENBQUMsQ0FBQyxLQUFJLENBQUMsU0FBUyxDQUFDLENBQUMsSUFBSSxDQUFDLFdBQVcsQ0FBQyxDQUFDO1FBQy9DLENBQUMsQ0FBQTtRQUVTLGdCQUFXLEdBQUc7WUFDcEIsT0FBTztnQkFDSCxNQUFNLEVBQUUsWUFBWTtnQkFDcEIsV0FBVyxFQUFFLEVBQUUsTUFBTSxFQUFFLFNBQVMsRUFBRSxNQUFNLEVBQUUsSUFBSSxFQUFFO2dCQUNoRCxjQUFjLEVBQUUsQ0FBQyxFQUFFLFNBQVMsRUFBRSxTQUFTLEVBQUUsQ0FBQztnQkFDMUMsU0FBUyxFQUFFLENBQUMsQ0FBQyxLQUFLLEVBQUUsRUFBRSxNQUFNLEVBQUUsQ0FBQyxFQUFFLENBQUMsRUFBRSxDQUFDLE9BQU8sQ0FBQyxDQUFDO2dCQUM5QyxrQkFBa0IsRUFBRTtvQkFDaEIsQ0FBQyxPQUFPLEVBQUUsRUFBRSxLQUFLLEVBQUUsRUFBRSxFQUFFLE1BQU0sRUFBRSxFQUFFLEVBQUUsUUFBUSxFQUFFLENBQUMsQ0FBQyxFQUFFLENBQUM7aUJBQ3JEO2dCQUNELGVBQWUsRUFBRSxDQUFDO2FBQ3JCLENBQUE7UUFDTCxDQUFDLENBQUM7UUFFUSwwQkFBcUIsR0FBRztZQUM5QixPQUFPLE9BQU8sQ0FBQyxXQUFXLENBQUM7Z0JBQ3ZCLFdBQVcsRUFBRSxFQUFFLE1BQU0sRUFBRSxTQUFTLEVBQUUsTUFBTSxFQUFFLElBQUksRUFBRTtn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QkFBaUIsQ0FBQyxPQUFPLENBQUMsQ0FBQztnQkFFaEMsa0NBQWtDO2dCQUNsQyxnQkFBZ0IsR0FBRyxLQUFJLENBQUMsbUJBQW1CLEVBQUUsQ0FBQztnQkFFOUMsc0NBQXNDO2dCQUN0QyxnQkFBZ0IsQ0FBQyxJQUFJLEVBQUUsQ0FBQztnQkFFeEIsS0FBSSxDQUFDLGtCQUFrQixFQUFFLENBQUM7WUFDOUIsQ0FBQyxDQUFDLENBQUM7WUFFSCxLQUFJLENBQUMsZUFBZSxHQUFHLE9BQU8sQ0FBQztRQUNuQyxDQUFDLENBQUMsQ0FBQzs7SUFDUCxDQUFDO0lBS0wscUJBQUM7QUFBRCxDQWxGQSxBQWtGQyxDQWxGNEIsY0FBYyxHQWtGMUM7QUFFRCxDQUFDLENBQUMsRUFBRSxDQUFDLGNBQWMsR0FBRztJQUNsQixJQUFJLENBQUMsSUFBSSxDQUFDLFVBQUMsS0FBSyxFQUFFLE9BQU87UUFDckIsSUFBSSxRQUFRLEdBQUcsQ0FBQyxDQUFDLE9BQU8sQ0FBQyxDQUFDO1FBQzFCLElBQUksWUFBWSxHQUEyQixRQUFRLENBQUMsSUFBSSxDQUFDLGVBQWUsQ0FBQyxDQUFDO1FBRTFFLFFBQVEsQ0FBQyxJQUFJLENBQUMsZ0JBQWdCLEVBQUUsSUFBSSxjQUFjLENBQUMsT0FBTyxFQUFFLFlBQVksQ0FBQyxDQUFDLENBQUM7SUFDL0UsQ0FBQyxDQUFDLENBQUM7SUFFSCxPQUFPLElBQUksQ0FBQztBQUNoQixDQUFDLENBQUM7QUFFRixDQUFDLENBQUMsUUFBUSxDQUFDLENBQUMsS0FBSyxDQUFDO0lBQ2QsSUFBTSxjQUFjLEdBQW1CLENBQUMsQ0FBQyxrQkFBa0IsQ0FBQyxDQUFDLGNBQWMsRUFBRSxDQUFDLElBQUksQ0FBQyxnQkFBZ0IsQ0FBQyxDQUFDO0FBQ3pHLENBQUMsQ0FBQyxDQUFDIiwiZmlsZSI6Im9yY2hhcmQud29ya2Zsb3dzLXZpZXdlci5qcyIsInNvdXJjZXNDb250ZW50IjpbIi8vLzxyZWZlcmVuY2UgcGF0aD0nLi4vTGliL2pxdWVyeS90eXBpbmdzLmQudHMnIC8+XG4vLy88cmVmZXJlbmNlIHBhdGg9Jy4uL0xpYi9qc3BsdW1iL3R5cGluZ3MuZC50cycgLz5cbi8vLzxyZWZlcmVuY2UgcGF0aD0nLi93b3JrZmxvdy1tb2RlbHMudHMnIC8+XG5cbmFic3RyYWN0IGNsYXNzIFdvcmtmbG93Q2FudmFzIHtcbiAgICBwcml2YXRlIG1pbkNhbnZhc0hlaWdodDogbnVtYmVyID0gNDAwO1xuXG4gICAgY29uc3RydWN0b3IocHJvdGVjdGVkIGNvbnRhaW5lcjogSFRNTEVsZW1lbnQsIHByb3RlY3RlZCB3b3JrZmxvd1R5cGU6IFdvcmtmbG93cy5Xb3JrZmxvd1R5cGUpIHtcbiAgICB9XG5cbiAgICBwcm90ZWN0ZWQgZ2V0QWN0aXZpdHlFbGVtZW50cyA9ICgpOiBKUXVlcnkgPT4ge1xuICAgICAgICByZXR1cm4gJCh0aGlzLmNvbnRhaW5lcikuZmluZCgnLmFjdGl2aXR5Jyk7XG4gICAgfVxuXG4gICAgcHJvdGVjdGVkIGdldERlZmF1bHRzID0gKCkgPT4ge1xuICAgICAgICByZXR1cm4ge1xuICAgICAgICAgICAgQW5jaG9yOiBcIkNvbnRpbnVvdXNcIixcbiAgICAgICAgICAgIERyYWdPcHRpb25zOiB7IGN1cnNvcjogJ3BvaW50ZXInLCB6SW5kZXg6IDIwMDAgfSxcbiAgICAgICAgICAgIEVuZHBvaW50U3R5bGVzOiBbeyBmaWxsU3R5bGU6ICcjMjI1NTg4JyB9XSxcbiAgICAgICAgICAgIEVuZHBvaW50czogW1tcIkRvdFwiLCB7IHJhZGl1czogNyB9XSwgW1wiQmxhbmtcIl1dLFxuICAgICAgICAgICAgQ29ubmVjdGlvbk92ZXJsYXlzOiBbXG4gICAgICAgICAgICAgICAgW1wiQXJyb3dcIiwgeyB3aWR0aDogMTIsIGxlbmd0aDogMTIsIGxvY2F0aW9uOiAtNSB9XSxcbiAgICAgICAgICAgIF0sXG4gICAgICAgICAgICBDb25uZWN0b3JaSW5kZXg6IDVcbiAgICAgICAgfVxuICAgIH07XG5cbiAgICBwcm90ZWN0ZWQgY3JlYXRlSnNQbHVtYkluc3RhbmNlID0gKCkgPT4ge1xuICAgICAgICByZXR1cm4ganNQbHVtYi5nZXRJbnN0YW5jZSh7XG4gICAgICAgICAgICBEcmFnT3B0aW9uczogeyBjdXJzb3I6ICdwb2ludGVyJywgekluZGV4OiAyMDAwIH0sXG4gICAgICAgICAgICBDb25uZWN0aW9uT3ZlcmxheXM6IFtcbiAgICAgICAgICAgICAgICBbJ0Fycm93Jywge1xuICAgICAgICAgICAgICAgICAgICBsb2NhdGlvbjogMSxcbiAgICAgICAgICAgICAgICAgICAgdmlzaWJsZTogdHJ1ZSxcbiAgICAgICAgICAgICAgICAgICAgd2lkdGg6IDExLFxuICAgICAgICAgICAgICAgICAgICBsZW5ndGg6IDExXG4gICAgICAgICAgICAgICAgfV0sXG4gICAgICAgICAgICAgICAgWydMYWJlbCcsIHtcbiAgICAgICAgICAgICAgICAgICAgbG9jYXRpb246IDAuNSxcbiAgICAgICAgICAgICAgICAgICAgaWQ6ICdsYWJlbCcsXG4gICAgICAgICAgICAgICAgICAgIGNzc0NsYXNzOiAnY29ubmVjdGlvbi1sYWJlbCdcbiAgICAgICAgICAgICAgICB9XVxuICAgICAgICAgICAgXSxcbiAgICAgICAgICAgIENvbnRhaW5lcjogdGhpcy5jb250YWluZXJcbiAgICAgICAgfSk7XG4gICAgfTtcblxuICAgIHByb3RlY3RlZCBnZXRFbmRwb2ludENvbG9yID0gKGFjdGl2aXR5OiBXb3JrZmxvd3MuQWN0aXZpdHkpID0+IHtcbiAgICAgICAgcmV0dXJuIGFjdGl2aXR5LmlzQmxvY2tpbmcgfHwgYWN0aXZpdHkuaXNTdGFydCA/ICcjN2FiMDJjJyA6IGFjdGl2aXR5LmlzRXZlbnQgPyAnIzNhOGFjZCcgOiAnIzdhYjAyYyc7XG4gICAgfVxuXG4gICAgcHJvdGVjdGVkIGdldFNvdXJjZUVuZHBvaW50T3B0aW9ucyA9IChhY3Rpdml0eTogV29ya2Zsb3dzLkFjdGl2aXR5LCBvdXRjb21lOiBXb3JrZmxvd3MuT3V0Y29tZSk6IEVuZHBvaW50T3B0aW9ucyA9PiB7XG4gICAgICAgIC8vIFRoZSBkZWZpbml0aW9uIG9mIHNvdXJjZSBlbmRwb2ludHMuXG4gICAgICAgIGNvbnN0IHBhaW50Q29sb3IgPSB0aGlzLmdldEVuZHBvaW50Q29sb3IoYWN0aXZpdHkpO1xuICAgICAgICByZXR1cm4ge1xuICAgICAgICAgICAgZW5kcG9pbnQ6ICdEb3QnLFxuICAgICAgICAgICAgYW5jaG9yOiAnQ29udGludW91cycsXG4gICAgICAgICAgICBwYWludFN0eWxlOiB7XG4gICAgICAgICAgICAgICAgc3Ryb2tlOiBwYWludENvbG9yLFxuICAgICAgICAgICAgICAgIGZpbGw6IHBhaW50Q29sb3IsXG4gICAgICAgICAgICAgICAgcmFkaXVzOiA3LFxuICAgICAgICAgICAgICAgIHN0cm9rZVdpZHRoOiAxXG4gICAgICAgICAgICB9LFxuICAgICAgICAgICAgaXNTb3VyY2U6IHRydWUsXG4gICAgICAgICAgICBjb25uZWN0b3I6IFsnRmxvd2NoYXJ0JywgeyBzdHViOiBbNDAsIDYwXSwgZ2FwOiAwLCBjb3JuZXJSYWRpdXM6IDUsIGFsd2F5c1Jlc3BlY3RTdHViczogdHJ1ZSB9XSxcbiAgICAgICAgICAgIGNvbm5lY3RvclN0eWxlOiB7XG4gICAgICAgICAgICAgICAgc3Ryb2tlV2lkdGg6IDIsXG4gICAgICAgICAgICAgICAgc3Ryb2tlOiAnIzk5OTk5OScsXG4gICAgICAgICAgICAgICAgam9pbnN0eWxlOiAncm91bmQnLFxuICAgICAgICAgICAgICAgIG91dGxpbmVTdHJva2U6ICd3aGl0ZScsXG4gICAgICAgICAgICAgICAgb3V0bGluZVdpZHRoOiAyXG4gICAgICAgICAgICB9LFxuICAgICAgICAgICAgaG92ZXJQYWludFN0eWxlOiB7XG4gICAgICAgICAgICAgICAgZmlsbDogJyMyMTY0NzcnLFxuICAgICAgICAgICAgICAgIHN0cm9rZTogJyMyMTY0NzcnXG4gICAgICAgICAgICB9LFxuICAgICAgICAgICAgY29ubmVjdG9ySG92ZXJTdHlsZToge1xuICAgICAgICAgICAgICAgIHN0cm9rZVdpZHRoOiAzLFxuICAgICAgICAgICAgICAgIHN0cm9rZTogJyMyMTY0NzcnLFxuICAgICAgICAgICAgICAgIG91dGxpbmVXaWR0aDogNSxcbiAgICAgICAgICAgICAgICBvdXRsaW5lU3Ryb2tlOiAnd2hpdGUnXG4gICAgICAgICAgICB9LFxuICAgICAgICAgICAgY29ubmVjdG9yT3ZlcmxheXM6IFtbJ0xhYmVsJywgeyBsb2NhdGlvbjogWzMsIC0xLjVdLCBjc3NDbGFzczogJ2VuZHBvaW50U291cmNlTGFiZWwnIH1dXSxcbiAgICAgICAgICAgIGRyYWdPcHRpb25zOiB7fSxcbiAgICAgICAgICAgIHV1aWQ6IGAke2FjdGl2aXR5LmlkfS0ke291dGNvbWUubmFtZX1gLFxuICAgICAgICAgICAgcGFyYW1ldGVyczoge1xuICAgICAgICAgICAgICAgIG91dGNvbWU6IG91dGNvbWVcbiAgICAgICAgICAgIH1cbiAgICAgICAgfTtcbiAgICB9O1xuXG4gICAgcHJvdGVjdGVkIGdldEFjdGl2aXR5ID0gZnVuY3Rpb24gKGlkOiBzdHJpbmcsIGFjdGl2aXRpZXM6IEFycmF5PFdvcmtmbG93cy5BY3Rpdml0eT4gPSBudWxsKTogV29ya2Zsb3dzLkFjdGl2aXR5IHtcbiAgICAgICAgaWYgKCFhY3Rpdml0aWVzKSB7XG4gICAgICAgICAgICBhY3Rpdml0aWVzID0gdGhpcy53b3JrZmxvd1R5cGUuYWN0aXZpdGllcztcbiAgICAgICAgfVxuICAgICAgICByZXR1cm4gJC5ncmVwKGFjdGl2aXRpZXMsICh4OiBXb3JrZmxvd3MuQWN0aXZpdHkpID0+IHguaWQgPT09IGlkKVswXTtcbiAgICB9XG5cbiAgICBwcm90ZWN0ZWQgdXBkYXRlQ29ubmVjdGlvbnMgPSAocGx1bWJlcjoganNQbHVtYkluc3RhbmNlKSA9PiB7XG4gICAgICAgIHZhciB3b3JrZmxvd0lkOiBudW1iZXIgPSB0aGlzLndvcmtmbG93VHlwZS5pZDtcblxuICAgICAgICAvLyBDb25uZWN0IGFjdGl2aXRpZXMuXG4gICAgICAgIGZvciAobGV0IHRyYW5zaXRpb25Nb2RlbCBvZiB0aGlzLndvcmtmbG93VHlwZS50cmFuc2l0aW9ucykge1xuICAgICAgICAgICAgY29uc3Qgc291cmNlRW5kcG9pbnRVdWlkOiBzdHJpbmcgPSBgJHt0cmFuc2l0aW9uTW9kZWwuc291cmNlQWN0aXZpdHlJZH0tJHt0cmFuc2l0aW9uTW9kZWwuc291cmNlT3V0Y29tZU5hbWV9YDtcbiAgICAgICAgICAgIGNvbnN0IHNvdXJjZUVuZHBvaW50OiBFbmRwb2ludCA9IHBsdW1iZXIuZ2V0RW5kcG9pbnQoc291cmNlRW5kcG9pbnRVdWlkKTtcbiAgICAgICAgICAgIGNvbnN0IGRlc3RpbmF0aW9uRWxlbWVudElkOiBzdHJpbmcgPSBgYWN0aXZpdHktJHt3b3JrZmxvd0lkfS0ke3RyYW5zaXRpb25Nb2RlbC5kZXN0aW5hdGlvbkFjdGl2aXR5SWR9YDtcblxuICAgICAgICAgICAgcGx1bWJlci5jb25uZWN0KHtcbiAgICAgICAgICAgICAgICBzb3VyY2U6IHNvdXJjZUVuZHBvaW50LFxuICAgICAgICAgICAgICAgIHRhcmdldDogZGVzdGluYXRpb25FbGVtZW50SWRcbiAgICAgICAgICAgIH0pO1xuICAgICAgICB9XG4gICAgfVxuXG4gICAgcHJvdGVjdGVkIHVwZGF0ZUNhbnZhc0hlaWdodCA9IGZ1bmN0aW9uICgpIHtcbiAgICAgICAgY29uc3QgJGNvbnRhaW5lciA9ICQodGhpcy5jb250YWluZXIpO1xuXG4gICAgICAgIC8vIEdldCB0aGUgYWN0aXZpdHkgZWxlbWVudCB3aXRoIHRoZSBoaWdoZXN0IFkgY29vcmRpbmF0ZS5cbiAgICAgICAgY29uc3QgJGFjdGl2aXR5RWxlbWVudHMgPSAkY29udGFpbmVyLmZpbmQoXCIuYWN0aXZpdHlcIik7XG4gICAgICAgIGxldCBjdXJyZW50RWxlbWVudFRvcCA9IDA7XG4gICAgICAgIGxldCBjdXJyZW50QWN0aXZpdHlIZWlnaHQgPSAwO1xuXG4gICAgICAgIGZvciAobGV0IGFjdGl2aXR5RWxlbWVudCBvZiAkYWN0aXZpdHlFbGVtZW50cy50b0FycmF5KCkpIHtcbiAgICAgICAgICAgIGNvbnN0ICRhY3Rpdml0eUVsZW1lbnQgPSAkKGFjdGl2aXR5RWxlbWVudCk7XG4gICAgICAgICAgICBjb25zdCB0b3AgPSAkYWN0aXZpdHlFbGVtZW50LnBvc2l0aW9uKCkudG9wO1xuXG4gICAgICAgICAgICBpZiAodG9wID4gY3VycmVudEVsZW1lbnRUb3ApIHtcbiAgICAgICAgICAgICAgICBjdXJyZW50RWxlbWVudFRvcCA9IHRvcDtcbiAgICAgICAgICAgICAgICBjdXJyZW50QWN0aXZpdHlIZWlnaHQgPSAkYWN0aXZpdHlFbGVtZW50LmhlaWdodCgpO1xuICAgICAgICAgICAgfVxuICAgICAgICB9XG5cbiAgICAgICAgbGV0IG5ld0NhbnZhc0hlaWdodCA9IGN1cnJlbnRFbGVtZW50VG9wICsgY3VycmVudEFjdGl2aXR5SGVpZ2h0O1xuICAgICAgICBjb25zdCBlbGVtZW50Qm90dG9tID0gY3VycmVudEVsZW1lbnRUb3AgKyBjdXJyZW50QWN0aXZpdHlIZWlnaHQ7XG4gICAgICAgIGNvbnN0IHN0cmV0Y2hWYWx1ZSA9IDEwMDtcblxuICAgICAgICBpZiAobmV3Q2FudmFzSGVpZ2h0IC0gZWxlbWVudEJvdHRvbSA8PSBzdHJldGNoVmFsdWUpIHtcbiAgICAgICAgICAgIG5ld0NhbnZhc0hlaWdodCArPSBzdHJldGNoVmFsdWU7XG4gICAgICAgIH1cblxuICAgICAgICAkY29udGFpbmVyLmhlaWdodChNYXRoLm1heCh0aGlzLm1pbkNhbnZhc0hlaWdodCwgbmV3Q2FudmFzSGVpZ2h0KSk7XG4gICAgfTtcbn0iLCIvLy88cmVmZXJlbmNlIHBhdGg9Jy4uL0xpYi9qcXVlcnkvdHlwaW5ncy5kLnRzJyAvPlxuLy8vPHJlZmVyZW5jZSBwYXRoPScuLi9MaWIvanNwbHVtYi90eXBpbmdzLmQudHMnIC8+XG4vLy88cmVmZXJlbmNlIHBhdGg9Jy4vd29ya2Zsb3ctbW9kZWxzLnRzJyAvPlxuLy8vPHJlZmVyZW5jZSBwYXRoPScuL3dvcmtmbG93LWNhbnZhcy50cycgLz5cblxuY2xhc3MgV29ya2Zsb3dWaWV3ZXIgZXh0ZW5kcyBXb3JrZmxvd0NhbnZhcyB7XG4gICAgcHJpdmF0ZSBqc1BsdW1iSW5zdGFuY2U6IGpzUGx1bWJJbnN0YW5jZTtcblxuICAgIGNvbnN0cnVjdG9yKHByb3RlY3RlZCBjb250YWluZXI6IEhUTUxFbGVtZW50LCBwcm90ZWN0ZWQgd29ya2Zsb3dUeXBlOiBXb3JrZmxvd3MuV29ya2Zsb3dUeXBlKSB7XG4gICAgICAgIHN1cGVyKGNvbnRhaW5lciwgd29ya2Zsb3dUeXBlKTtcbiAgICAgICAgY29uc3Qgc2VsZiA9IHRoaXM7XG5cbiAgICAgICAganNQbHVtYi5yZWFkeSgoKSA9PiB7XG4gICAgICAgICAgICBqc1BsdW1iLmltcG9ydERlZmF1bHRzKHRoaXMuZ2V0RGVmYXVsdHMoKSk7XG5cbiAgICAgICAgICAgIGNvbnN0IHBsdW1iZXIgPSB0aGlzLmNyZWF0ZUpzUGx1bWJJbnN0YW5jZSgpO1xuXG4gICAgICAgICAgICAvLyBMaXN0ZW4gZm9yIG5ldyBjb25uZWN0aW9ucy5cbiAgICAgICAgICAgIHBsdW1iZXIuYmluZCgnY29ubmVjdGlvbicsIGZ1bmN0aW9uIChjb25uSW5mbywgb3JpZ2luYWxFdmVudCkge1xuICAgICAgICAgICAgICAgIGNvbnN0IGNvbm5lY3Rpb246IENvbm5lY3Rpb24gPSBjb25uSW5mby5jb25uZWN0aW9uO1xuICAgICAgICAgICAgICAgIGNvbnN0IG91dGNvbWU6IFdvcmtmbG93cy5PdXRjb21lID0gY29ubmVjdGlvbi5nZXRQYXJhbWV0ZXJzKCkub3V0Y29tZTtcblxuICAgICAgICAgICAgICAgIGNvbnN0IGxhYmVsOiBhbnkgPSBjb25uZWN0aW9uLmdldE92ZXJsYXkoJ2xhYmVsJyk7XG4gICAgICAgICAgICAgICAgbGFiZWwuc2V0TGFiZWwob3V0Y29tZS5kaXNwbGF5TmFtZSk7XG4gICAgICAgICAgICB9KTtcblxuICAgICAgICAgICAgbGV0IGFjdGl2aXR5RWxlbWVudHMgPSB0aGlzLmdldEFjdGl2aXR5RWxlbWVudHMoKTtcblxuICAgICAgICAgICAgdmFyIGFyZUVxdWFsT3V0Y29tZXMgPSBmdW5jdGlvbiAob3V0Y29tZXMxOiBXb3JrZmxvd3MuT3V0Y29tZVtdLCBvdXRjb21lczI6IFdvcmtmbG93cy5PdXRjb21lW10pOiBib29sZWFuIHtcbiAgICAgICAgICAgICAgICBpZiAob3V0Y29tZXMxLmxlbmd0aCAhPSBvdXRjb21lczIubGVuZ3RoKSB7XG4gICAgICAgICAgICAgICAgICAgIHJldHVybiBmYWxzZTtcbiAgICAgICAgICAgICAgICB9XG5cbiAgICAgICAgICAgICAgICBmb3IgKGxldCBpID0gMDsgaSA8IG91dGNvbWVzMS5sZW5ndGg7IGkrKykge1xuICAgICAgICAgICAgICAgICAgICBjb25zdCBvdXRjb21lMSA9IG91dGNvbWVzMVtpXTtcbiAgICAgICAgICAgICAgICAgICAgY29uc3Qgb3V0Y29tZTIgPSBvdXRjb21lczJbaV07XG5cbiAgICAgICAgICAgICAgICAgICAgaWYgKG91dGNvbWUxLm5hbWUgIT0gb3V0Y29tZTIuZGlzcGxheU5hbWUgfHwgb3V0Y29tZTEuZGlzcGxheU5hbWUgIT0gb3V0Y29tZTIuZGlzcGxheU5hbWUpIHtcbiAgICAgICAgICAgICAgICAgICAgICAgIHJldHVybiBmYWxzZTtcbiAgICAgICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgIH1cblxuICAgICAgICAgICAgICAgIHJldHVybiB0cnVlO1xuICAgICAgICAgICAgfVxuXG4gICAgICAgICAgICAvLyBTdXNwZW5kIGRyYXdpbmcgYW5kIGluaXRpYWxpemUuXG4gICAgICAgICAgICBwbHVtYmVyLmJhdGNoKCgpID0+IHtcbiAgICAgICAgICAgICAgICB2YXIgd29ya2Zsb3dJZDogbnVtYmVyID0gdGhpcy53b3JrZmxvd1R5cGUuaWQ7XG5cbiAgICAgICAgICAgICAgICBhY3Rpdml0eUVsZW1lbnRzLmVhY2goKGluZGV4LCBhY3Rpdml0eUVsZW1lbnQpID0+IHtcbiAgICAgICAgICAgICAgICAgICAgY29uc3QgJGFjdGl2aXR5RWxlbWVudCA9ICQoYWN0aXZpdHlFbGVtZW50KTtcbiAgICAgICAgICAgICAgICAgICAgY29uc3QgYWN0aXZpdHlJZCA9ICRhY3Rpdml0eUVsZW1lbnQuZGF0YSgnYWN0aXZpdHktaWQnKTtcbiAgICAgICAgICAgICAgICAgICAgY29uc3QgYWN0aXZpdHkgPSB0aGlzLmdldEFjdGl2aXR5KGFjdGl2aXR5SWQpO1xuXG4gICAgICAgICAgICAgICAgICAgIC8vIENvbmZpZ3VyZSB0aGUgYWN0aXZpdHkgYXMgYSB0YXJnZXQuXG4gICAgICAgICAgICAgICAgICAgIHBsdW1iZXIubWFrZVRhcmdldChhY3Rpdml0eUVsZW1lbnQsIHtcbiAgICAgICAgICAgICAgICAgICAgICAgIGRyb3BPcHRpb25zOiB7IGhvdmVyQ2xhc3M6ICdob3ZlcicgfSxcbiAgICAgICAgICAgICAgICAgICAgICAgIGFuY2hvcjogJ0NvbnRpbnVvdXMnLFxuICAgICAgICAgICAgICAgICAgICAgICAgZW5kcG9pbnQ6IFsnQmxhbmsnLCB7IHJhZGl1czogOCB9XVxuICAgICAgICAgICAgICAgICAgICB9KTtcblxuICAgICAgICAgICAgICAgICAgICAvLyBBZGQgc291cmNlIGVuZHBvaW50cy5cbiAgICAgICAgICAgICAgICAgICAgZm9yIChsZXQgb3V0Y29tZSBvZiBhY3Rpdml0eS5vdXRjb21lcykge1xuICAgICAgICAgICAgICAgICAgICAgICAgY29uc3Qgc291cmNlRW5kcG9pbnRPcHRpb25zID0gdGhpcy5nZXRTb3VyY2VFbmRwb2ludE9wdGlvbnMoYWN0aXZpdHksIG91dGNvbWUpO1xuICAgICAgICAgICAgICAgICAgICAgICAgcGx1bWJlci5hZGRFbmRwb2ludChhY3Rpdml0eUVsZW1lbnQsIHsgY29ubmVjdG9yT3ZlcmxheXM6IFtbJ0xhYmVsJywgeyBsYWJlbDogb3V0Y29tZS5kaXNwbGF5TmFtZSwgY3NzQ2xhc3M6ICdjb25uZWN0aW9uLWxhYmVsJyB9XV0gfSwgc291cmNlRW5kcG9pbnRPcHRpb25zKTtcbiAgICAgICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgIH0pO1xuXG4gICAgICAgICAgICAgICAgLy8gQ29ubmVjdCBhY3Rpdml0aWVzLlxuICAgICAgICAgICAgICAgIHRoaXMudXBkYXRlQ29ubmVjdGlvbnMocGx1bWJlcik7XG5cbiAgICAgICAgICAgICAgICAvLyBSZS1xdWVyeSB0aGUgYWN0aXZpdHkgZWxlbWVudHMuXG4gICAgICAgICAgICAgICAgYWN0aXZpdHlFbGVtZW50cyA9IHRoaXMuZ2V0QWN0aXZpdHlFbGVtZW50cygpO1xuXG4gICAgICAgICAgICAgICAgLy8gTWFrZSBhbGwgYWN0aXZpdHkgZWxlbWVudHMgdmlzaWJsZS5cbiAgICAgICAgICAgICAgICBhY3Rpdml0eUVsZW1lbnRzLnNob3coKTtcblxuICAgICAgICAgICAgICAgIHRoaXMudXBkYXRlQ2FudmFzSGVpZ2h0KCk7XG4gICAgICAgICAgICB9KTtcblxuICAgICAgICAgICAgdGhpcy5qc1BsdW1iSW5zdGFuY2UgPSBwbHVtYmVyO1xuICAgICAgICB9KTtcbiAgICB9XG5cbiAgICBwcm90ZWN0ZWQgZ2V0RW5kcG9pbnRDb2xvciA9IChhY3Rpdml0eTogV29ya2Zsb3dzLkFjdGl2aXR5KSA9PiB7XG4gICAgICAgIHJldHVybiBhY3Rpdml0eS5pc0Jsb2NraW5nID8gJyM3YWIwMmMnIDogYWN0aXZpdHkuaXNFdmVudCA/ICcjM2E4YWNkJyA6ICcjN2FiMDJjJztcbiAgICB9XG59XG5cbiQuZm4ud29ya2Zsb3dWaWV3ZXIgPSBmdW5jdGlvbiAodGhpczogSlF1ZXJ5KTogSlF1ZXJ5IHtcbiAgICB0aGlzLmVhY2goKGluZGV4LCBlbGVtZW50KSA9PiB7XG4gICAgICAgIHZhciAkZWxlbWVudCA9ICQoZWxlbWVudCk7XG4gICAgICAgIHZhciB3b3JrZmxvd1R5cGU6IFdvcmtmbG93cy5Xb3JrZmxvd1R5cGUgPSAkZWxlbWVudC5kYXRhKCd3b3JrZmxvdy10eXBlJyk7XG5cbiAgICAgICAgJGVsZW1lbnQuZGF0YSgnd29ya2Zsb3dWaWV3ZXInLCBuZXcgV29ya2Zsb3dWaWV3ZXIoZWxlbWVudCwgd29ya2Zsb3dUeXBlKSk7XG4gICAgfSk7XG5cbiAgICByZXR1cm4gdGhpcztcbn07XG5cbiQoZG9jdW1lbnQpLnJlYWR5KGZ1bmN0aW9uICgpIHtcbiAgICBjb25zdCB3b3JrZmxvd1ZpZXdlcjogV29ya2Zsb3dWaWV3ZXIgPSAkKCcud29ya2Zsb3ctY2FudmFzJykud29ya2Zsb3dWaWV3ZXIoKS5kYXRhKCd3b3JrZmxvd1ZpZXdlcicpO1xufSk7Il19
